using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using UnityEngine;
using MEC;

namespace AmongSCP
{
    public static class Util
    {
        public static int VoteAmount;
        
        public static IEnumerator<float> CallEmergencyMeeting(Player caller, string message, bool isBodyReport)
        {
            Log.Debug("Emergency meeting invoked.");
            var meetingStarted = true;
            var meetingTime = isBodyReport ? AmongSCP.Singleton.Config.BodyReportedMeetingTime : AmongSCP.Singleton.Config.EmergencyTime;

            var votePos = AmongSCP.Singleton.Config.VotePosition.GetPositions();

            foreach (var ply in Player.List)
            {
                ply.Position = votePos;
            }

            while (meetingStarted)
            {
                Exiled.API.Features.Map.ShowHint(
                    $"Meeting ends in {meetingTime} seconds. " +
                    $"\n {VoteAmount} Votes " +
                    $"\n Alive Players: {EventHandlers.PlayerManager.AlivePlayers.Count}", 1);
                    //$" \n Dead Players: {EventHandlers.PlayerManager.DeadPlayer.Count}", 1);
                    
                yield return Timing.WaitForSeconds(1f);
                
                meetingTime -= 1;
                
                if (meetingTime >= 1) continue;
                meetingStarted = false;
                
                PointManager.SpawnPlayers(EventHandlers.PlayerManager.AlivePlayers.ToArray());
            }
        }
    }
}