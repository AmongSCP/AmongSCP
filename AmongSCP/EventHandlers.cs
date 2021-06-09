﻿using System;
using AmongSCP.Map;
using Exiled.API.Enums;
using Exiled.Events.EventArgs;
using MEC;
using Exiled.API.Extensions;
using Exiled.API.Features;
using System.Linq;

namespace AmongSCP
{
    public static class EventHandlers
    {
        internal static TaskManager TaskManager = new TaskManager();
        
        internal static PlayerManager.PlayerManager PlayerManager = new PlayerManager.PlayerManager();

        internal static SpawnInteractables SpawnInteractables;

        public static bool ImposterCanKill;
        
        private static bool _starting = false;

        public static void Reset()
        {
            TaskManager = new TaskManager();
            PlayerManager = new PlayerManager.PlayerManager();
        }
        
        public static void OnPickupItem(PickingUpItemEventArgs ev)
        {
            if (ev.Pickup.gameObject.TryGetComponent<InteractableBehavior>(out var component))
            {
                ev.IsAllowed = component.Interactable.OnInteract(ev.Player);
            }
        }

        public static void OnDying(DyingEventArgs ev)
        {
            ev.Target.Items.Clear();
        }
        
        public static void OnDied(DiedEventArgs ev)
        {
            Log.Debug($"{ev.Target} died at: {ev.Target.Position.x}, {ev.Target.Position.y}, {ev.Target.Position.z}. Role was {ev.Target.Role}");

            var plyinfo = ev.Target.GetInfo();
            plyinfo.IsAlive = false;
            plyinfo.Role = global::AmongSCP.PlayerManager.Role.None;

            //TODO - I didnt see checks anywhere to check round end conditions but remove this if there is
            if (PlayerManager.Crewmates.Count <= PlayerManager.Imposters.Count)
                Round.ForceEnd();
        }

        public static void OnRagdollSpawn(SpawningRagdollEventArgs ev)
        {
            if (ev.Owner?.Nickname == null) return;

            SpawnInteractables.SpawnDeadBodyInteractable(ev.Position, ev.Owner.Nickname);
        }

        public static void OnRoleChanging(ChangingRoleEventArgs ev)
        {
            if (ev.NewRole == RoleType.Tutorial || ev.NewRole == RoleType.Spectator)
            {
                var plyinfo = ev.Player.GetInfo();
                plyinfo.IsAlive = false;
                plyinfo.Role = global::AmongSCP.PlayerManager.Role.None;

                return;
            }

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
                Util.SetUpDoors();
                Util.RemoveAllItems();
                SpawnInteractables = new SpawnInteractables();
                PlayerManager.UpdateQueueNoWait();

                var players = PlayerManager.PickPlayers(AmongSCP.Singleton.Config.MaxPlayers);
                players.ShuffleListSecure();

                PlayerManager.ClearQueued();

                for (var i = 0; i < players.Length; i++)
                {
                    var info = players[i].GetInfo();
                    info.IsAlive = true;
                    
                    if (i % 5 < AmongSCP.Singleton.Config.ImposterRatio)
                    {
                        info.Role = global::AmongSCP.PlayerManager.Role.Imposter;
                        players[i].Role = AmongSCP.Singleton.Config.ImposterRole;
                    }
                    else
                    {
                        info.Role = global::AmongSCP.PlayerManager.Role.Crewmate;
                        players[i].Role = AmongSCP.Singleton.Config.CrewmateRole;
                    }
                }

                Timing.CallDelayed(.1f, () =>
                {
                    foreach (var player in players)
                    {
                        Util.ChangeOutfit(player, AmongSCP.Singleton.Config.CrewmateRole, PlayerManager);
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

                        TaskManager.SplitTasks();
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
            TaskManager.EndGame();
            PlayerManager.EndGame();
        }

        public static void OnPlayerShooting(ShootingEventArgs ev)
        {
            if (!PlayerManager.Imposters.Contains(ev.Shooter))
            {
                ev.IsAllowed = false;
                return;
            }

            var lastShot = ev.Shooter.GetInfo().LastShot;
            var seconds = lastShot == DateTime.MinValue ? 31 : (int) DateTime.Now.Subtract(lastShot).TotalSeconds;

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
            if (!PlayerManager.Imposters.Contains(ev.Shooter) || ev.HitboxTypeEnum > HitBoxType.HEAD || Util.meetingStarted)
            {
                ev.Damage = 0;
                ev.CanHurt = false;
                return;
            }
            
            ev.Damage = 200f;
            ev.Shooter.GetInfo().LastShot = DateTime.Now;
        }

        public static void OnElevatorUsed(InteractingElevatorEventArgs ev)
        {
            if (ev.Lift.Type() == ElevatorType.Scp049 || ev.Lift.Type() == ElevatorType.Nuke) return;

            Log.Debug("OnElevatorUsed() invoked.");
            ev.IsAllowed = false;
        }

        //Task Event Handlers
        public static void OnOpeningGenerator(UnlockingGeneratorEventArgs ev)
        {
            if (!TaskManager.TryCompletingTask(ev.Player, TaskType.Generator)) return;

            MirrorExtensions.SendFakeSyncVar(ev.Player, ev.Generator.netIdentity, typeof(Generator079), nameof(Generator079.NetworkisDoorUnlocked), true);
            MirrorExtensions.SendFakeSyncVar(ev.Player, ev.Generator.netIdentity, typeof(Generator079), nameof(Generator079.NetworkisDoorOpen), true);
        }
    }
}