using System;
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
            //Log.Debug("Method OnPickupItem() invoked.");

            
            //Log.Debug(ev.Pickup.gameObject.TryGetComponent<SCPStats.Hats.HatPlayerComponent>(out var w));
            if (ev.Pickup.gameObject.TryGetComponent<InteractableBehavior>(out var component))
            {
                ev.IsAllowed = component.Interactable.OnInteract(ev.Player);
            }
            
            ev.IsAllowed = false;
        }

        public static void OnDying(DyingEventArgs ev)
        {
            ev.Target.Items.Clear();
            if (ev.Target.GetInfo().Role == global::AmongSCP.PlayerManager.Role.Imposter) return;
            TaskManager.DeletePlayerTasks(ev.Target);
        }
        
        public static void OnDied(DiedEventArgs ev)
        {
            Log.Debug($"{ev.Target} died at: {ev.Target.Position.x}, {ev.Target.Position.y}, {ev.Target.Position.z}. Role was {ev.Target.Role}");

            var plyinfo = ev.Target.GetInfo();
            plyinfo.IsAlive = false;
            plyinfo.Role = global::AmongSCP.PlayerManager.Role.None;

            if (EventHandlers.PlayerManager.Crewmates.Count <= EventHandlers.PlayerManager.Imposters.Count)
            {
                Exiled.API.Features.Map.Broadcast((ushort)5f, "Imposters win!");
                Round.ForceEnd();
            }
            else if(TaskManager.AllTasksCompleted() || EventHandlers.PlayerManager.Imposters.Count == 0)
            {
                Exiled.API.Features.Map.Broadcast((ushort)5f, "Crewmates Win!!");
                Round.ForceEnd();
            }
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
                SpawnInteractables = new SpawnInteractables();
                TaskManager = new TaskManager();
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
                        players[i].Broadcast((ushort)3f,"You are an Imposter!");
                    }
                    else
                    {
                        info.Role = global::AmongSCP.PlayerManager.Role.Crewmate;
                        players[i].Role = AmongSCP.Singleton.Config.CrewmateRole;
                        players[i].Broadcast((ushort)3f, "You are a crewmate!");
                        players[i].Broadcast((ushort)15f, "Interact with all 5 generators to complete your tasks.");
                    }
                }

                Timing.CallDelayed(.1f, () =>
                {
                    SpawnInteractables.SpawnHats(players);
                    foreach (var player in players)
                    {
                        Util.ChangeOutfit(player, AmongSCP.Singleton.Config.CrewmateRole, PlayerManager);
                        player.Inventory.Clear();
                    }
                    
                    foreach (var imposter in PlayerManager.Imposters)
                    {
                        imposter.Ammo[(int) AmmoType.Nato9] = 999;
                        imposter.Inventory.AddNewItem(ItemType.GunUSP);
                        imposter.Inventory.AddNewItem(ItemType.GrenadeFlash);
                        imposter.Inventory.AddNewItem(ItemType.GrenadeFrag);
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
            switch (ev.Item.id)
            {
                case ItemType.GrenadeFlash:
                    //Log.Debug("Grenade Flash is being called.");
                    Util.ModifyLightIntensity(0);
                    break;
                case ItemType.GrenadeFrag when Util.CanNuke:
                    //Log.Debug("Warhead is being called.");
                    Util.RunDetonateWarhead();
                    break;
                case ItemType.Adrenaline:
                    ev.Player.Position = new UnityEngine.Vector3(77.2f, -998.67f, 199.3183f);
                    break;
            }

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
            var seconds = lastShot == DateTime.MinValue ? (AmongSCP.Singleton.Config.KillCooldown + 1) : (int) DateTime.Now.Subtract(lastShot).TotalSeconds;

            if (seconds > AmongSCP.Singleton.Config.KillCooldown)
            {
                return;
            }

            ev.IsAllowed = false;
            ev.Shooter.Broadcast((ushort)3f, "You are still on cooldown. " + (AmongSCP.Singleton.Config.KillCooldown - seconds) + " seconds left.");
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

            //Log.Debug("OnElevatorUsed() invoked.");
            ev.IsAllowed = false;
        }

        public static void OnTriggeringTeslaEvent(TriggeringTeslaEventArgs ev)
        {
            if (AmongSCP.Singleton.Config.TeslaGatesEnabled) return;
            ev.IsTriggerable = false;
        }

        public static void OnChangingLeverStatusEvent(ChangingLeverStatusEventArgs ev)
        {
            /*
            if(!Warhead.LeverStatus)
            {
                Util.StopWarhead();
            }
            */
        }

        public static void OnThrowingGrenade(ThrowingGrenadeEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        //Task Event Handlers
        public static void OnOpeningGenerator(UnlockingGeneratorEventArgs ev)
        {
            if (ev.Player.GetInfo().InteractedGenerators.Contains(ev.Generator)) return;
            if (!TaskManager.TryCompletingTask(ev.Player, TaskType.Generator)) return;
            ev.Player.GetInfo().InteractedGenerators.Add(ev.Generator);
            MirrorExtensions.SendFakeSyncVar(ev.Player, ev.Generator.netIdentity, typeof(Generator079), nameof(Generator079.NetworkisDoorUnlocked), true);
            MirrorExtensions.SendFakeSyncVar(ev.Player, ev.Generator.netIdentity, typeof(Generator079), nameof(Generator079.NetworkisDoorOpen), true);
        }
    }
}