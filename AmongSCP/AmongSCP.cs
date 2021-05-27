using System.Diagnostics.Eventing;

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
        
        //TODO - Use this to mark items for TODO and make comments on it
        //BUG - Use this to mark bugs with short description
        
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
            
            Log.Info("Registered Events");
        }

        private void UnRegisterEvents()
        {
            ServerEvent.RoundStarted -= _eventHandlers.OnGameStart;
            
            Log.Info("Unregistered Events");
        }
        
    }
}