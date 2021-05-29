﻿using System.ComponentModel;
using Exiled.API.Interfaces;
using UnityEngine;

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

        [Description("The number of emergncy meetings for each person.")]
        public int EmergencyMeetings = 2;

        [Description("Item that when dropped will call an emergency meeting.")]
        public ItemType EmergencyMeetingTrigger = ItemType.Disarmer;

        [Description("Length of time a player can vent for")]
        public float VentTime = 10;

        [Description("Time after an imposter vents that they cannot kill")]
        public float VentKillCooldown = 5;

        //I think this should work?
        [Description("Location when someone calls a emergency meeting or voting starts")]
        public Vector3 VotePosition = new Vector3(54, 1020, -45);
    }
}