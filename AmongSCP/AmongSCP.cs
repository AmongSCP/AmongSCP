using HarmonyLib;
using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using PlayerEvent = Exiled.Events.Handlers.Player;
using ServerEvent = Exiled.Events.Handlers.Server;
using System.Collections.Generic;
using System.Linq;
using Exiled.Loader;
using MEC;


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
            Singleton = this;
            Timing.RunCoroutine(DisableOtherPlugins());
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
            PlayerEvent.ThrowingItem += EventHandlers.OnThrowingGrenade;
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
            PlayerEvent.ThrowingItem -= EventHandlers.OnThrowingGrenade;
        }

        private IEnumerator<float> DisableOtherPlugins()
        {
            yield return Timing.WaitForSeconds(1f);
            
            // First, disable every plugin but AmongSCP.
            foreach (var plugin in Loader.Plugins.ToList())
            {
                if (plugin.Name == "AmongSCP") continue;
                
                try
                {
                    plugin.OnUnregisteringCommands();
                    plugin.OnDisabled();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
            
            yield return Timing.WaitForSeconds(1f);

            // Now, re-enable every plugin.
            List<string> pluginsToEnable = new List<string>()
            {
                "SCPStats",
                "DiscordIntegration"
            };

            foreach(var plugin in Loader.Plugins.ToList())
            {
                if (!pluginsToEnable.Contains(plugin.Name) && !plugin.Name.StartsWith("Exiled") &&
                    !plugin.Prefix.StartsWith("exiled") &&
                    !plugin.Assembly.GetName().Name.StartsWith("Exiled")) continue;
                
                try
                {
                    plugin.OnEnabled();
                    plugin.OnRegisteringCommands();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
    }
}