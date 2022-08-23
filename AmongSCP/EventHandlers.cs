using System;
using AmongSCP.Map;
using Exiled.API.Enums;
using Exiled.Events.EventArgs;
using MEC;
using Exiled.API.Extensions;
using Exiled.API.Features;
using System.Linq;
using Exiled.Loader;
using MapGeneration.Distributors;
using RoundRestarting;

namespace AmongSCP
{
    public static class EventHandlers
    {
        internal static PlayerManager.PlayerManager PlayerManager = new PlayerManager.PlayerManager();

        internal static SpawnInteractables SpawnInteractables;

        public static bool ImposterCanKill;
        
        private static bool _starting = false;

        public static void Reset()
        {
            PlayerManager = new PlayerManager.PlayerManager();
        }
        
        public static void OnPickupItem(PickingUpItemEventArgs ev)
        {
            //Log.Debug("Method OnPickupItem() invoked.");

            
            //Log.Debug(ev.Pickup.gameObject.TryGetComponent<SCPStats.Hats.HatPlayerComponent>(out var w));
            if (ev.Pickup.Base.gameObject.TryGetComponent<InteractableBehavior>(out var component))
            {
                ev.IsAllowed = component.Interactable.OnInteract(ev.Player);
            }
            
            ev.IsAllowed = false;
        }

        public static void OnDying(DyingEventArgs ev)
        {
            ev.Target.ClearInventory();
            if (ev.Target.GetInfo().Role == global::AmongSCP.PlayerManager.Role.Imposter) return;
            if(Player.List.Contains(ev.Target))
            {
                TaskManager.DeletePlayerTasks(ev.Target);
            }
            
        }
        
        public static void OnDied(DiedEventArgs ev)
        {
            Log.Debug($"{ev.Target} died at: {ev.Target.Position.x}, {ev.Target.Position.y}, {ev.Target.Position.z}. Role was {ev.Target.Role}", AmongSCP.Singleton.Config.showLogs);

            var plyinfo = ev.Target.GetInfo();
            plyinfo.IsAlive = false;
            plyinfo.Role = global::AmongSCP.PlayerManager.Role.None;
            PlayerManager.ReloadLists();
            if (EventHandlers.PlayerManager.Crewmates.Count <= EventHandlers.PlayerManager.Imposters.Count)
            {
                Exiled.API.Features.Map.Broadcast((ushort)5f, "Imposters win!");
                Timing.CallDelayed(5f, RoundRestart.InitiateRoundRestart);
            }
            else if(TaskManager.AllTasksCompleted() || EventHandlers.PlayerManager.Imposters.Count == 0)
            {
                Exiled.API.Features.Map.Broadcast((ushort)5f, "Crewmates Win!!");
                Timing.CallDelayed(5f, RoundRestart.InitiateRoundRestart);
            }
        }

        public static void OnRagdollSpawn(SpawningRagdollEventArgs ev)
        {
            if (ev.Owner?.Nickname == null || ev.Role.GetTeam() == Team.CHI) return;
            SpawnInteractables.SpawnDeadBodyInteractable(new MapPosition(ev.Position), ev.Nickname);
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

            ev.IsAllowed = false;
        }

        public static void OnRoundEnding(EndingRoundEventArgs ev)
        {
            if (_starting) ev.IsAllowed = false;
        }

        public static void OnGameStart()
        {
            Loader.Plugins.FirstOrDefault(pl => pl.Name == "ScpStats")?.Assembly?.GetType("SCPStats.EventHandler")?.GetField("PauseRound")?.SetValue(null, true);
            
            _starting = true;
            Timing.CallDelayed(.2f, () =>
            {
                try
                {
                    Util.initialCooldownOn = true;
                    Timing.CallDelayed(AmongSCP.Singleton.Config.initialCooldown, () => Util.initialCooldownOn = false);

                    Util.SetUpDoors();
                    SpawnInteractables = new SpawnInteractables();
                    TaskManager.AddPossibleTasks();
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
                            players[i].SetRole(AmongSCP.Singleton.Config.ImposterRole);
                            players[i].Broadcast((ushort) 3f, "You are an Imposter! +" +
                                                              "\n Kill all Crewmates to win!");
                            players[i].ShowHint(
                                "You have a gun with a cooldown and two grendades which when thrown turn off lights and turn on the nuke.",
                                5f);
                        }
                        else
                        {
                            info.Role = global::AmongSCP.PlayerManager.Role.Crewmate;
                            players[i].SetRole(AmongSCP.Singleton.Config.CrewmateRole);
                            players[i].Broadcast((ushort) 3f, "You are a crewmate! +" +
                                                              "\n Interact with all 5 generators to complete your tasks.");
                        }
                    }

                    Timing.CallDelayed(.2f, () =>
                    {
                        try
                        {
                            SpawnInteractables.SpawnHats(players);
                            foreach (var player in players)
                            {
                                Util.ChangeOutfit(player.NetworkIdentity, player.Role.Type);
                                player.ClearInventory();
                                player.InfoArea = player.InfoArea & ~PlayerInfoArea.PowerStatus &
                                                  ~PlayerInfoArea.UnitName;
                            }

                            foreach (var player in PlayerManager.Crewmates)
                            {
                                player.AddItem(ItemType.Flashlight);
                            }

                            foreach (var imposter in PlayerManager.Imposters)
                            {
                                imposter.Ammo[ItemType.Ammo9x19] = 10;
                                imposter.AddItem(ItemType.GunCOM18);
                                imposter.AddItem(ItemType.GrenadeFlash);
                                imposter.AddItem(ItemType.GrenadeHE);
                                imposter.AddItem(ItemType.Flashlight);
                            }

                            Timing.CallDelayed(.3f, () =>
                            {
                                try
                                {
                                    PointManager.SpawnPlayers(players);
                                    TaskManager.SplitTasks();
                                }
                                catch (Exception e)
                                {
                                    Log.Error(e);
                                }
                            });
                            
                            Timing.CallDelayed(1f, () =>
                            {
                                try
                                {
                                    PointManager.SpawnPlayers(players);
                                    _starting = false;
                                }
                                catch (Exception e)
                                {
                                    Log.Error(e);
                                }
                            });
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                        }
                    });
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
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

        public static void OnGameEnd(RoundEndedEventArgs ev)
        {
            TaskManager.EndGame();
            PlayerManager.EndGame();
        }

        public static void OnPlayerShooting(ShootingEventArgs ev)
        {
            if (!PlayerManager.Imposters.Contains(ev.Shooter) || Util.initialCooldownOn)
            {
                ev.Shooter.ShowHint("Wait a second!");
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
            ev.Shooter.Ammo[ItemType.Ammo9x19] = 10;
            
            if (!PlayerManager.Imposters.Contains(ev.Shooter) || Util.meetingStarted)
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
            if (ev.Lift.Type == ElevatorType.Scp049 || ev.Lift.Type == ElevatorType.Nuke) return;

            //Log.Debug("OnElevatorUsed() invoked.");
            ev.IsAllowed = false;
        }

        public static void OnTriggeringTeslaEvent(TriggeringTeslaEventArgs ev)
        {
            if (AmongSCP.Singleton.Config.TeslaGatesEnabled) return;
            ev.IsTriggerable = false;
        }

        public static void OnThrowingGrenade(ThrowingItemEventArgs ev)
        {
            ev.IsAllowed = false;
            
            if (ev.Item.Type != ItemType.GrenadeFlash && ev.Item.Type != ItemType.GrenadeHE) return;
            
            if (Util.initialCooldownOn)
            {
                ev.Player.ShowHint("Wait a sec!");
                return;
            }

            switch (ev.Item.Type)
            {
                case ItemType.GrenadeFlash:
                    //Log.Debug("Grenade Flash is being called.");
                    Util.ModifyLightIntensity(0, ev.Player);
                    break;
                case ItemType.GrenadeHE:
                    Util.RunDetonateWarhead(ev.Player);
                    break;
            }
        }

        public static void OnOpeningGenerator(UnlockingGeneratorEventArgs ev)
        {
            var info = ev.Player.GetInfo();

            if (info.CompletedTasks.Contains(ev.Generator) || !TaskManager.TryCompletingTask(ev.Player, TaskType.Generator)) return;

            info.CompletedTasks.Add(ev.Generator);

            MirrorExtensions.SendFakeSyncVar(ev.Player, ev.Generator.Base.netIdentity, typeof(Scp079Generator), nameof(Scp079Generator.Network_flags), ev.Generator.Base.Network_flags | (byte) Scp079Generator.GeneratorFlags.Open | (byte) Scp079Generator.GeneratorFlags.Unlocked);
        }

        public static void OnItemDrop(DroppingItemEventArgs ev)
        {
            if(ev.Player.GetInfo().skipped)
            {
                ev.Player.Broadcast(1, "You have already skipped!");
                ev.IsAllowed = false;
                return;
            }
            if (Util.meetingStarted)
            {
                if (ev.Item.Type == ItemType.Flashlight)
                {
                    ev.Player.GetInfo().hasVoted = true;
                    Util.VoteAmount++;
                    ev.Player.Broadcast((ushort)5f, "You have Skipped!");
                    ev.Player.GetInfo().skipped = true;
                }
            }
            ev.IsAllowed = false;
        }
    }
}