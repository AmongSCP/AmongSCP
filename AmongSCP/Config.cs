using System.ComponentModel;
using Exiled.API.Interfaces;
using Exiled.CustomItems.API.Features;
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
        public int EmergencyMeetings { get; set; } = 5;

        [Description("Item that when dropped will call an emergency meeting.")]
        public ItemType EmergencyMeetingTrigger { get; set; } = ItemType.Disarmer;

        [Description("Length of time a player can vent for.")]
        public float VentTime { get; set; } = 10;

        [Description("Location when someone calls a emergency meeting or voting starts. Tutorial tower position is 54, 1020, -45.")]
        public Vector VotePosition { get; set; } = new Vector(54, 1020, -45);

        [Description("The distance you have to be from a body to be able to trigger a report.")]
        public float MaxReportDistance { get; set; } = 2f;

        [Description("Number of tasks each crewmate gets.")]
        public int CrewmateTasks { get; set; } = 5;

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

        //Messages
        public string ImposterSpawnMessage { get; set; } = "You are an Imposter! " +
                                                        "\n Kill all Crewmates to win!";

        public string ImposterItemsHint { get; set; } = "You have a gun with a cooldown and two grendades which when thrown turn off lights and turn on the nuke.";

        public string CrewmteSpawnMessage { get; set; } = "You are a crewmate! " +
                                                         "\n Interact with all 5 generators to complete your tasks.";

        public string ImpostersWinMessage { get; set; } = "Imposters win!";

        public string CrewmateWinMessage { get; set; } = "Crewmates Win!!";

        public string CoolDownMessage { get; set; } = " seconds left of your cooldown!";

        public string SkipMessage { get; set; } = "You have Skipped!";

        public string InMeetingMessage { get; set; } = "You are in a meeting!";

        public string NukeActiveMessage { get; set; } = "Nuke is active!";

        public string CoolDownNoCountMessage { get; set; } = "You are on cooldown!";

        public string LightsOffMessage { get; set; } = "Lights are off!";

        public string FixLightsMessage { get; set; } = "Fix Lights in Micro!";
    }
}