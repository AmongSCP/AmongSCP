﻿using System.ComponentModel;
using Exiled.API.Interfaces;

namespace AmongSCP
{
    public class Config : IConfig
    {
        [Description("Is the plugin enabled?")]
        public bool IsEnabled { get; set; } = true;

        [Description("Number of imposters for every 5 people.")]
        public int ImposterRatio { get; set; } = 1;

        [Description("Maximum amount of players in a game.")]
        public int MaxPlayers { get; set; } = 10;
        
        [Description("The role of the imposter.")]
        public RoleType ImposterRole { get; set; } = RoleType.ChaosInsurgency;
        
        [Description("The role of the crewmate.")]
        public RoleType CrewmateRole { get; set; } = RoleType.NtfCommander;
    }
}