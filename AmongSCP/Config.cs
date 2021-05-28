using System.ComponentModel;
using Exiled.API.Interfaces;

namespace AmongSCP
{
    public class Config : IConfig
    {
        [Description("Is the plugin enabled?")]
        public bool IsEnabled { get; set; } = true;

        [Description("Number of imposters for every 5 people")]
        public int Imposters { get; set; } = 2;
    }
}