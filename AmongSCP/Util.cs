using Exiled.API.Features;
using UnityEngine;
using MEC;

namespace AmongSCP
{
    public static class Util
    {
        public static void CallEmergencyMeeting(Player caller, string message)
        {
            foreach (var ply in Player.List)
            {
                ply.Position = AmongSCP.Singleton.Config.VotePosition; 
            }
            
            Log.Debug("Emergency meeting invoked.");
        }

        public static void ReportBody()
        {

        }
    }
}