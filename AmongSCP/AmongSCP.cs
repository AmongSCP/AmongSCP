namespace AmongSCP
{
    using System;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    
    using PlayerEvent = Exiled.Events.Handlers.Player;
    using ServerEvent = Exiled.Events.Handlers.Server;
    using MapEvent = Exiled.Events.Handlers.Map;
    
    public class AmongSCP : Plugin<Config>
    {
        public override string Author { get; } = "Parkeymon, PintTheDragon, RedRanger26";
        public override string Name { get; } = "AmongSCP";
        public override Version Version { get; } = new Version(1, 0, 0);
        public override PluginPriority Priority { get; } = PluginPriority.First;

        private EventHandlers _eventHandlers;

        public override void OnEnabled()
        {
            _eventHandlers = new EventHandlers(this);
         
            RegisterEvents();
            
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            UnRegisterEvents();
            
            _eventHandlers = null;

            base.OnDisabled();
        }

        private void RegisterEvents()
        {
            ServerEvent.RoundStarted += _eventHandlers.OnGameStart;

            PlayerEvent.Died += _eventHandlers.OnDied;
            PlayerEvent.PickingUpItem += _eventHandlers.OnPickupItem;
            PlayerEvent.Verified += _eventHandlers.OnJoin;
            PlayerEvent.Left += _eventHandlers.OnLeave;
        }

        private void UnRegisterEvents()
        {
            ServerEvent.RoundStarted -= _eventHandlers.OnGameStart;
            
            PlayerEvent.Died -= _eventHandlers.OnDied;
            PlayerEvent.PickingUpItem -= _eventHandlers.OnPickupItem;
            PlayerEvent.Verified -= _eventHandlers.OnJoin;
            PlayerEvent.Left -= _eventHandlers.OnLeave;
        }
    }
}