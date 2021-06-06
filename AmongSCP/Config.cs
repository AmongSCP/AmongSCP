using System.ComponentModel;
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
        public int EmergencyMeetings { get; set; } = 2;

        [Description("Item that when dropped will call an emergency meeting.")]
        public ItemType EmergencyMeetingTrigger { get; set; } = ItemType.Disarmer;

        [Description("Length of time a player can vent for.")]
        public float VentTime { get; set; } = 10;

        [Description("Time after an imposter vents that they cannot kill.")]
        public float VentKillCooldown { get; set; } = 5;
        
        [Description("Location when someone calls a emergency meeting or voting starts. Tutorial tower position is 54, 1020, -45.")]
        public Position VotePosition { get; set; } = new Position()
        {
            X = 54,
            Y = 1020,
            Z = -45
        };

        [Description("The distance you have to be from a body to be able to trigger a report.")]
        public float MaxReportDistance { get; set; } = 2f;

        [Description("Number of tasks each crewmate gets.")]
        public int CrewmateTasks { get; set; } = 5;

        [Description("The amount of time an emergency meeting lasts for.")]
        public int EmergencyTime = 30; 
        
        [Description("Body Reporting Time")]
        public int BodyReportedMeetingTime = 30; 
    }
}