using System.Collections.Generic;
using Exiled.API.Features;
using MEC;
using Exiled.API.Extensions;
using Interactables.Interobjects.DoorUtils;
using UnityEngine;

namespace AmongSCP
{
    public static class Util
    {
        public static int VoteAmount;

        public static bool meetingStarted = false;

        public static bool CanTurnOffLights = true;

        public static bool CanNuke = true;

        public static float curLightIntensity = 1;

        public static IEnumerator<float> CallEmergencyMeeting(Player caller, string message, bool isBodyReport)
        {
            meetingStarted = true;
            var meetingTime = isBodyReport ? AmongSCP.Singleton.Config.BodyReportedMeetingTime : AmongSCP.Singleton.Config.EmergencyTime;

            var votePos = AmongSCP.Singleton.Config.VotePosition.GetPositions();

            Exiled.API.Features.Map.ShowHint(message, 5f);

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
            caller.GetInfo().CalledEmergencyMeeting = false;
        }

        public static void ModifyLightIntensity(float intensity)
        {
            if ((!CanTurnOffLights && intensity == 0) || Warhead.IsInProgress || meetingStarted) return;
           
            foreach (Room room in Exiled.API.Features.Map.Rooms)
            {
                room.SetLightIntensity(intensity);
                curLightIntensity = intensity;
            }
            
            
            CanTurnOffLights = false;
            Timing.CallDelayed(AmongSCP.Singleton.Config.LightsCooldown, () => CanTurnOffLights = true);
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

        public static void RunDetonateWarhead()
        {
            if (Warhead.IsInProgress || curLightIntensity != 1 || meetingStarted) return;
            Timing.RunCoroutine(DetonateWarhead());
        }

        public static IEnumerator<float> DetonateWarhead()
        {
            Warhead.DetonationTimer = 90;
            Warhead.Start();
            Warhead.LeverStatus = true;

            Log.Debug("Warhead: " + Warhead.IsInProgress.ToString());
            while(Warhead.IsInProgress)
            {
                if(Warhead.DetonationTimer <= 9f)
                {
                    Log.Debug("Warhead is Locked");
                    foreach(Player ply in EventHandlers.PlayerManager.Imposters)
                    {
                        ply.Position = Exiled.API.Features.Map.GetRandomSpawnPoint(RoleType.ChaosInsurgency);
                    }
                    break;
                }
                yield return Timing.WaitForOneFrame;
            }
            CanNuke = false;
            Timing.CallDelayed(AmongSCP.Singleton.Config.NukeCooldown, () => CanNuke = true);
        }

        public static void RemoveAllItems()
        {
            
        }
    }
}