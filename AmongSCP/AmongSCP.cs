using System;
using Exiled.API.Enums;
using Exiled.API.Features;

namespace AmongSCP
{
    public class AmongSCP : Plugin<Config>
    {
        public override string Name { get; } = "AmongSCP";
        public override Version Version { get; } = new Version(1, 0, 0);
        public override PluginPriority Priority { get; } = PluginPriority.First;

        public static AmongSCP Singleton;

        public override void OnEnabled()
        {
            Singleton = this;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Singleton = null;
            base.OnDisabled();
        }
    }
}