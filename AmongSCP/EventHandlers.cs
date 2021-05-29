using AmongSCP.Map;
using Exiled.API.Enums;
using Exiled.Events.EventArgs;
using MEC;

namespace AmongSCP
{
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using System.Collections.Generic;
    using System.Linq;

    public static class EventHandlers
    {
        internal static PlayerManager PlayerManager = new PlayerManager();

        private static int _emergencyMeetings;

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
            
        }

        //TODO - Figure out how to report bodies
        public static void ReportBody()
        {
            
        }

        public static void OnDying(DyingEventArgs ev)
        {
            ev.Target.Items.Clear();
        }
        
        public static void OnDied(DiedEventArgs ev)
        {
           Log.Debug($"Someone died at: {ev.Target.Position.x}, {ev.Target.Position.y}, {ev.Target.Position.z}");
            /*if(!PlayerManager.Imposters.Contains(ev.Target))
            {
                PlayerManager.DeadPlayers.Add(ev.Target, ev.Target.Position);
            }*/
        }

        public static void OnRoleChanging(ChangingRoleEventArgs ev)
        {
            if (ev.NewRole == RoleType.Tutorial) return;

            if ((ev.NewRole != AmongSCP.Singleton.Config.CrewmateRole || PlayerManager.Crewmates.Contains(ev.Player)) && (ev.NewRole != AmongSCP.Singleton.Config.ImposterRole || PlayerManager.Imposters.Contains(ev.Player))) return;

            ev.IsEscaped = false;
            ev.NewRole = ev.Player.Role;
            ev.ShouldPreservePosition = true;

            ev.Items.Clear();
            ev.Items.AddRange(ev.Player.Items.Select(item => item.id));
        }
        
        public static void OnGameStart()
        {
            Timing.CallDelayed(.2f, () =>
            {
                PlayerManager.UpdateQueueNoWait();

                var players = PlayerManager.PickPlayers(AmongSCP.Singleton.Config.MaxPlayers);

                PlayerManager.ClearQueued();

                players.ShuffleList();

                PlayerManager.Crewmates.Clear();
                PlayerManager.Imposters.Clear();

                for (var i = 0; i < players.Length; i++)
                {
                    if (i % 5 < AmongSCP.Singleton.Config.ImposterRatio)
                    {
                        players[i].SetRole(AmongSCP.Singleton.Config.ImposterRole);
                        PlayerManager.Imposters.Add(players[i]);
                    }
                    else
                    {
                        players[i].SetRole(AmongSCP.Singleton.Config.CrewmateRole);
                        PlayerManager.Crewmates.Add(players[i]);
                    }
                }

                Timing.CallDelayed(.1f, () =>
                {
                    foreach (var imposter in PlayerManager.Imposters)
                    {
                        imposter.Ammo[(int) AmmoType.Nato9] = 999;
                        imposter.Inventory.AddNewItem(ItemType.GunUSP);

                        ChangeOutfit(imposter, AmongSCP.Singleton.Config.CrewmateRole);
                    }
                    
                    foreach (var crewmate in PlayerManager.Crewmates)
                    {
                        ChangeOutfit(crewmate, AmongSCP.Singleton.Config.CrewmateRole);
                    }

                    Timing.CallDelayed(.1f, () =>
                    {
                        PointManager.SpawnPlayers(players);
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
        }

        private static void ChangeOutfit(Player ply, RoleType type)
        {
            foreach (var target in Player.List)
            {
                if(!PlayerManager.Imposters.Contains(target))
                {
                    MirrorExtensions.SendFakeSyncVar(target, ply.ReferenceHub.networkIdentity, typeof(CharacterClassManager), nameof(CharacterClassManager.NetworkCurClass), (sbyte)type);
                }
            }
        }
    }
}