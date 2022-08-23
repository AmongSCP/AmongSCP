using System.ComponentModel;
using Exiled.API.Interfaces;
using Exiled.CustomItems.API.Features;
using UnityEngine;
using Utf8Json.Internal.DoubleConversion;

namespace AmongSCP
{
    public class Config : IConfig
    {
        [Description("Is the plugin enabled?")]
        public bool IsEnabled { get; set; } = true;

        [Description("Should logs be shown?(Only enable if developing)")]
        public bool showLogs = true;

        [Description("Number of imposters for every 5 people.")]
        public int ImposterRatio { get; set; } = 1;

        [Description("Maximum amount of players in a game.")]
        public int MaxPlayers { get; set; } = 10;
        
        [Description("The role of the imposter.")]
        public RoleType ImposterRole { get; set; } = RoleType.ChaosConscript;
        
        [Description("The role of the crewmate.")]
        public RoleType CrewmateRole { get; set; } = RoleType.NtfCaptain;

        [Description("The number of emergncy meetings for each person.")]
        public int EmergencyMeetings { get; set; } = 5;

        [Description("Length of time a player can vent for.")]
        public float VentTime { get; set; } = 10;
        
        [Description("Location when someone calls a emergency meeting or voting starts. Tutorial tower position is 54, 1020, -45.")]
        public Vector3 VotePosition { get; set; } = new Vector3(54, 1020, -45);

        [Description("The distance you have to be from a body to be able to trigger a report.")]
        public float MaxReportDistance { get; set; } = 2f;

        [Description("Number of tasks each crewmate gets.")]
        public int CrewmateTasks { get; set; } = 3;

        [Description("The amount of time an emergency meeting lasts for.")]
        public int EmergencyTime { get; set; } = 30; 
        
        [Description("Body Reporting Time")]
        public int BodyReportedMeetingTime { get; set; } = 30;

        [Description("Cooldown for turning out lights.")]
        public int LightsCooldown { get; set; } = 30;

        [Description("Should tesla gates be turned on?")]
        public bool TeslaGatesEnabled { get; set; } = false;

        [Description("Cooldown for turning on the nuke.")]
        public int NukeCooldown { get; set; } = 10;

        [Description("Cooldown for the imposter to kill someone.")]
        public int KillCooldown { get; set; } = 30;

        [Description("Cooldown for calling an emergency meeting.")]
        public int globalEmergencyMeetingCooldown = 30;

        [Description("Initial round cooldown before you can do most things")]
        public int initialCooldown = 5;
    }
}