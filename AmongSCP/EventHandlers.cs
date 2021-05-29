using System;
using AmongSCP.Map;
using Exiled.API.Enums;
using Exiled.Events.EventArgs;
using MEC;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Interactables.Interobjects.DoorUtils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AmongSCP
{
    public static class EventHandlers
    {
        internal static PlayerManager PlayerManager = new PlayerManager();

        private static int _emergencyMeetings;

        public static bool ImposterCanKill;

        private static Vector3 _votingPosition = new Vector3();
        
        private static bool _starting = false;
        
        public static void Reset() 
        {
            PlayerManager = new PlayerManager();
        }
        
        public static void OnPickupItem(PickingUpItemEventArgs ev)
        {
            if (ev.Pickup.gameObject.TryGetComponent<InteractableBehavior>(out var component))
            {
                ev.IsAllowed = component.Interactable.OnInteract(ev.Player);
            }
        }
        
        //TODO - Work on voting
        private static void StartVoting()
        {
            _votingPosition = new Vector3(AmongSCP.Singleton.Config.VotePosition.x, AmongSCP.Singleton.Config.VotePosition.y, AmongSCP.Singleton.Config.VotePosition.z);
            foreach (var ply in Player.List)
            {
                ply.Position = _votingPosition;
            }
        }

        //TODO - Figure out how to report bodies
        public static void ReportBody(Player ply)
        {
            foreach (var pos in PlayerManager.DeadPositions)
            {
                if (Vector3.Distance(ply.Position, pos) <= AmongSCP.Singleton.Config.MaxReportDistance)
                {
                    Log.Debug("Player is close enough to report");
                    //StartVoting();
                }
            }
            
            
        }

        public static void OnDying(DyingEventArgs ev)
        {
            ev.Target.Items.Clear();
        }
        
        public static void OnDied(DiedEventArgs ev)
        {
           Log.Debug($"Someone died at: {ev.Target.Position.x}, {ev.Target.Position.y}, {ev.Target.Position.z}");

           PlayerManager.DeadPlayers.Add(ev.Target);
           if (PlayerManager.Imposters.Contains(ev.Target)) return;
           PlayerManager.DeadPositions.Add(ev.Target.Position);
        }

        public static void OnRoleChanging(ChangingRoleEventArgs ev)
        {
            if (ev.NewRole == RoleType.Tutorial) return;

            //if newrole == crewmate && player is crewmate OR newrole == imposter && player is imposter, return
            if ((ev.NewRole == AmongSCP.Singleton.Config.CrewmateRole && PlayerManager.Crewmates.Contains(ev.Player)) || (ev.NewRole == AmongSCP.Singleton.Config.ImposterRole && PlayerManager.Imposters.Contains(ev.Player))) return;

            ev.IsEscaped = false;
            ev.NewRole = ev.Player.Role;
            ev.ShouldPreservePosition = true;

            ev.Items.Clear();
            ev.Items.AddRange(ev.Player.Items.Select(item => item.id));
        }

        public static void OnRoundEnding(EndingRoundEventArgs ev)
        {
            if (_starting) ev.IsAllowed = false;
        }

        public static void OnGameStart()
        {
            _starting = true;
            Timing.CallDelayed(.1f, () =>
            {
                SetUpDoors();
                PlayerManager.UpdateQueueNoWait();

                var players = PlayerManager.PickPlayers(AmongSCP.Singleton.Config.MaxPlayers);

                PlayerManager.ClearQueued();

                players.ShuffleListSecure();

                for (var i = 0; i < players.Length; i++)
                {
                    if (i % 5 < AmongSCP.Singleton.Config.ImposterRatio)
                    {
                        PlayerManager.Imposters.Add(players[i]);
                        players[i].Role = AmongSCP.Singleton.Config.ImposterRole;
                    }
                    else
                    {
                        PlayerManager.Crewmates.Add(players[i]);
                        players[i].Role = AmongSCP.Singleton.Config.CrewmateRole;
                    }
                }

                Timing.CallDelayed(.1f, () =>
                {
                    foreach (var player in players)
                    {
                        ChangeOutfit(player, AmongSCP.Singleton.Config.CrewmateRole);
                        player.Inventory.Clear();
                    }
                    
                    foreach (var imposter in PlayerManager.Imposters)
                    {
                        imposter.Ammo[(int) AmmoType.Nato9] = 999;
                        imposter.Inventory.AddNewItem(ItemType.GunUSP);
                    }

                    Timing.CallDelayed(.1f, () =>
                    {
                        PointManager.SpawnPlayers(players);
                        
                        _starting = false;
                    });
                });
            });
        }

        public static void OnJoin(VerifiedEventArgs ev)
        {
            if (!RoundSummary.RoundInProgress()) return;

            PlayerManager.UpdateQueue();
        }

        public static void OnLeave(LeftEventArgs ev)
        {
            if (!RoundSummary.RoundInProgress()) return;

            PlayerManager.UpdateQueue();
        }

        public static void OnItemDrop(DroppingItemEventArgs ev)
        {
            if (!PlayerManager.Imposters.Contains(ev.Player) || ev.Item.id != ItemType.GunUSP) return;
            
            ev.IsAllowed = false;
        }

        public static void OnGameEnd(RoundEndedEventArgs ev)
        {
            PlayerManager.EndGame();
        }

        public static void OnPlayerShooting(ShootingEventArgs ev)
        {
            if (!PlayerManager.Imposters.Contains(ev.Shooter))
            {
                ev.IsAllowed = false;
                return;
            }

            var seconds = 31;

            if (PlayerManager.LastShot.TryGetValue(ev.Shooter, out var time))
            {
                seconds = (int) DateTime.Now.Subtract(time).TotalSeconds;
            }

            if (seconds > 30)
            {
                return;
            }

            ev.IsAllowed = false;
            ev.Shooter.ShowHint("You are still on cooldown. " + (30 - seconds) + " seconds left.");
        }

        public static void OnPlayerShoot(ShotEventArgs ev)
        {
            //Anything > HEAD is non-player.
            if (!PlayerManager.Imposters.Contains(ev.Shooter) || ev.HitboxTypeEnum > HitBoxType.HEAD)
            {
                ev.Damage = 0;
                ev.CanHurt = false;
                return;
            }
            
            ev.Damage = 200f;
            PlayerManager.LastShot[ev.Shooter] = DateTime.Now;
        }

        public static void OnElevatorUsed(InteractingElevatorEventArgs ev)
        {
            if(ev.Lift.Type() != ElevatorType.Scp049 || ev.Lift.Type() != ElevatorType.Nuke)
            {
                ev.IsAllowed = false;
            }
        }

        private static void ChangeOutfit(Player ply, RoleType type)
        {
            foreach (var target in Player.List)
            {
                //If the target is not an imposter, show the fake role, otherwise show the true role.
                if(!PlayerManager.Imposters.Contains(target))
                {
                    MirrorExtensions.SendFakeSyncVar(target, ply.ReferenceHub.networkIdentity, typeof(CharacterClassManager), nameof(CharacterClassManager.NetworkCurClass), (sbyte) type);
                }
                else
                {
                    MirrorExtensions.SendFakeSyncVar(target, ply.ReferenceHub.networkIdentity, typeof(CharacterClassManager), nameof(CharacterClassManager.NetworkCurClass), (sbyte) ply.Role);
                }
            }
        }

        private static void SetUpDoors()
        {
            foreach(var door in Exiled.API.Features.Map.Doors)
            {
                door.NetworkTargetState = true;
                door.ServerChangeLock(DoorLockReason.SpecialDoorFeature, true);
            }
        }
    }
}