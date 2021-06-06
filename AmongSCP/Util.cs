using System.Collections;
using System.Collections.Generic;
using Exiled.API.Features;
using UnityEngine;
using MEC;

namespace AmongSCP
{
    public static class Util
    {
        public static IEnumerator<float> CallEmergencyMeeting(Player caller, string message)
        {
            Log.Debug("Emergency meeting invoked.");
            var meetingStarted = true;
            var meetingTime = AmongSCP.Singleton.Config.EmergencyTime;
            
            foreach (var ply in Player.List)
            {
                ply.Position = AmongSCP.Singleton.Config.VotePosition; 
            }

            while (meetingStarted)
            {
                Exiled.API.Features.Map.ShowHint($"Meeting ends in {meetingTime} seconds.", 1);
                yield return Timing.WaitForSeconds(1f);
                meetingTime -= 1;
                if (meetingTime <= 1)
                {
                    meetingStarted = false;
                }

            }
        }

        public static void ReportBody()
        {
            
        }
    }
}