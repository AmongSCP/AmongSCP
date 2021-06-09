using System.Collections.Generic;
using Exiled.API.Features;
using MEC;
using Exiled.API.Extensions;
using Interactables.Interobjects.DoorUtils;

namespace AmongSCP
{
    public static class Util
    {
        public static int VoteAmount;

        public static bool meetingStarted = false;

        public static bool CanTurnOffLights = true;

        public static IEnumerator<float> CallEmergencyMeeting(Player caller, string message, bool isBodyReport)
        {
            Log.Debug("Emergency meeting invoked.");
            meetingStarted = true;
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

        public static void ModifyLightIntensity(float intensity)
        {
            foreach (Room room in Exiled.API.Features.Map.Rooms)
            {
                if(intensity!=0)
                {
                    room.SetLightIntensity(intensity);
                }
                else
                {
                    if(CanTurnOffLights)
                    {
                        room.SetLightIntensity(intensity);
                        CanTurnOffLights = false;
                        Timing.CallDelayed(AmongSCP.Singleton.Config.LightsCooldown, () => CanTurnOffLights = true);
                    }
                }
                
            }
        }

        public static void SetUpDoors()
        {
            foreach (var door in Exiled.API.Features.Map.Doors)
            {
                door.NetworkTargetState = true;
                door.ServerChangeLock(DoorLockReason.SpecialDoorFeature, true);
            }
        }


        public static void ChangeOutfit(Player ply, RoleType type, PlayerManager.PlayerManager playerManager)
        {
            foreach (var target in Player.List)
            {
                //If the target is not an imposter, show the fake role, otherwise show the true role.
                if (!playerManager.Imposters.Contains(target))
                {
                    MirrorExtensions.SendFakeSyncVar(target, ply.ReferenceHub.networkIdentity, typeof(CharacterClassManager), nameof(CharacterClassManager.NetworkCurClass), (sbyte)type);
                }
                else
                {
                    MirrorExtensions.SendFakeSyncVar(target, ply.ReferenceHub.networkIdentity, typeof(CharacterClassManager), nameof(CharacterClassManager.NetworkCurClass), (sbyte)ply.Role);
                }
            }
        }

        public static void RemoveAllItems()
        {
            //needs to be implemented
        }
    }
}