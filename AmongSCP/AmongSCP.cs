using HarmonyLib;
using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using PlayerEvent = Exiled.Events.Handlers.Player;
using ServerEvent = Exiled.Events.Handlers.Server;
using MapEvent = Exiled.Events.Handlers.Map;
using Warhead = Exiled.Events.Handlers.Warhead;
using System.Linq;

namespace AmongSCP
{
    public class AmongSCP : Plugin<Config>
    {
        public override string Author { get; } = "Parkeymon, PintTheDragon, RedRanger26";
        public override string Name { get; } = "AmongSCP";
        public override Version Version { get; } = new Version(1, 0, 1);
        public override PluginPriority Priority { get; } = PluginPriority.Last;
        public override string Prefix { get; } = "among_scp";

        public static AmongSCP Singleton;
        
        private Harmony _harmony;

        public override void OnEnabled()
        {
            DisableOtherPlugins();
            Singleton = this;

            RegisterEvents();

            _harmony = new Harmony("AmongSCP");
            _harmony.PatchAll();
            
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            UnRegisterEvents();
            
            _harmony.UnpatchAll();
            _harmony = null;

            EventHandlers.Reset();

            Singleton = null;

            base.OnDisabled();
        }

        private void RegisterEvents()
        {
            ServerEvent.RoundStarted += EventHandlers.OnGameStart;
            ServerEvent.RoundEnded += EventHandlers.OnGameEnd;
            ServerEvent.EndingRound += EventHandlers.OnRoundEnding;

            PlayerEvent.Died += EventHandlers.OnDied;
            PlayerEvent.PickingUpItem += EventHandlers.OnPickupItem;
            PlayerEvent.Verified += EventHandlers.OnJoin;
            PlayerEvent.Left += EventHandlers.OnLeave;
            PlayerEvent.ChangingRole += EventHandlers.OnRoleChanging;
            PlayerEvent.Dying += EventHandlers.OnDying;
            PlayerEvent.DroppingItem += EventHandlers.OnItemDrop;
            PlayerEvent.Shot += EventHandlers.OnPlayerShoot;
            PlayerEvent.Shooting += EventHandlers.OnPlayerShooting;
            PlayerEvent.InteractingElevator += EventHandlers.OnElevatorUsed;
            PlayerEvent.UnlockingGenerator += EventHandlers.OnOpeningGenerator;
            PlayerEvent.SpawningRagdoll += EventHandlers.OnRagdollSpawn;
            PlayerEvent.TriggeringTesla += EventHandlers.OnTriggeringTeslaEvent;
            PlayerEvent.ThrowingGrenade += EventHandlers.OnThrowingGrenade;
        }

        private void UnRegisterEvents()
        {
            ServerEvent.RoundStarted -= EventHandlers.OnGameStart;
            ServerEvent.RoundEnded -= EventHandlers.OnGameEnd;
            ServerEvent.EndingRound -= EventHandlers.OnRoundEnding;
            
            PlayerEvent.Died -= EventHandlers.OnDied;
            PlayerEvent.PickingUpItem -= EventHandlers.OnPickupItem;
            PlayerEvent.Verified -= EventHandlers.OnJoin;
            PlayerEvent.Left -= EventHandlers.OnLeave;
            PlayerEvent.ChangingRole -= EventHandlers.OnRoleChanging;
            PlayerEvent.Dying -= EventHandlers.OnDying;
            PlayerEvent.DroppingItem -= EventHandlers.OnItemDrop;
            PlayerEvent.Shot -= EventHandlers.OnPlayerShoot;
            PlayerEvent.Shooting -= EventHandlers.OnPlayerShooting;
            PlayerEvent.InteractingElevator -= EventHandlers.OnElevatorUsed;
            PlayerEvent.UnlockingGenerator -= EventHandlers.OnOpeningGenerator;
            PlayerEvent.SpawningRagdoll -= EventHandlers.OnRagdollSpawn;
            PlayerEvent.TriggeringTesla -= EventHandlers.OnTriggeringTeslaEvent;
            PlayerEvent.ThrowingGrenade -= EventHandlers.OnThrowingGrenade;
        }

        private void DisableOtherPlugins()
        {
            /*
            try
            {
                Exiled.Loader.Loader.Plugins.First(x => x.Name == "WaitAndChillReborn").OnDisabled();
            }
            catch (Exception e)
            {
                Log.Debug(e, AmongSCP.Singleton.Config.showLogs);
            }
            */
        }
    }
}