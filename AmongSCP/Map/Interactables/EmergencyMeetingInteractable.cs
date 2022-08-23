using MEC;
using Exiled.API.Features;
using System;
using CustomPlayerEffects;
using UnityEngine;

namespace AmongSCP.Map.Interactables
{
    public class EmergencyMeetingInteractable
    {
        private Interactable _interactable;

        public EmergencyMeetingInteractable(ItemData data)
        {
            _interactable = new Interactable(data, player =>
            {
                if (!Util.globalCanCallEmergencyMeeting || Util.initialCooldownOn)
                {
                    player.ShowHint($"You must wait a little bit!", 1f);
                    return;
                }

                if (Warhead.IsInProgress)
                {
                    player.ShowHint("Warhead is in progress!", 1f);
                    return;
                }

                if (Util.curLightIntensity == 0)
                {
                    player.ShowHint("Go fix Lights!", 1f);
                    return;
                }

                if (player.GetInfo().CalledEmergencyMeeting)
                {
                    player.ShowHint("You have already called an emergency meeting!", 1f);
                    return;
                }

                if(player.GetInfo().EmergencyMeetings == 0)
                {
                    player.ShowHint("You have used all of your emergency meetings!", 1f);
                    return;
                }

                try
                {
                    player.GetInfo().CalledEmergencyMeeting = true;
                    Timing.RunCoroutine(Util.CallEmergencyMeeting(player, player.Nickname + " has called an emergency meeting!", false));
                    player.GetInfo().EmergencyMeetings--;
                }
                catch (Exception e)
                {
                    Log.Debug(e, AmongSCP.Singleton.Config.showLogs);
                }
                
            }, false, true);
        }
    }
    
    public class MeetingEffect : PlayerEffect, IMovementSpeedEffect
    {
        public float GetMovementSpeed(float currentSpeed) => .1f * currentSpeed;

        public bool DisableSprint => true;
    }
}