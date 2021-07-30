using System;
using System.Collections.Generic;
using Exiled.API.Features;
using MEC;
using Exiled.API.Extensions;
using Interactables.Interobjects.DoorUtils;
using UnityEngine;
using Exiled.API.Enums;

namespace AmongSCP
{
    public static class Util
    {
        public static int VoteAmount = 0;

        public static bool meetingStarted = false;

        public static bool CanTurnOffLights = true;

        public static bool CanNuke = true;

        public static float curLightIntensity = 1;

        public static IEnumerator<float> CallEmergencyMeeting(Player caller, string message, bool isBodyReport)
        {
            Warhead.Stop();
            meetingStarted = true;
            var meetingTime = isBodyReport ? AmongSCP.Singleton.Config.BodyReportedMeetingTime : AmongSCP.Singleton.Config.EmergencyTime;

            var votePos = AmongSCP.Singleton.Config.VotePosition.ToVector3;

            Exiled.API.Features.Map.Broadcast((ushort)5f, message + "\n Interact with a persons hat to vote for them or drop your flashlight to skip!");

            foreach (var ply in Player.List)
            {
                if(ply.GetInfo().IsAlive) ply.Position = votePos;
            }

            while (meetingStarted)
            {
                Exiled.API.Features.Map.ShowHint(
                    $"Voting ends in {meetingTime} seconds. " +
                    $"\n {VoteAmount} Votes " +
                    $"\n Alive Players: {EventHandlers.PlayerManager.AlivePlayers.Count}", (ushort)1f);
                //$" \n Dead Players: {EventHandlers.PlayerManager.DeadPlayer.Count}", 1);

                yield return Timing.WaitForSeconds(1f);

                meetingTime--;

                if (meetingTime >= 1 && VoteAmount!=EventHandlers.PlayerManager.AlivePlayers.Count) continue;

                meetingStarted = false;
                PointManager.SpawnPlayers(EventHandlers.PlayerManager.AlivePlayers.ToArray());
                VoteAmount = 0;
                caller.GetInfo().CalledEmergencyMeeting = false;

                EventHandlers.PlayerManager.KillMostVotedPlayer();
                EventHandlers.PlayerManager.ClearAllPlayersVotes();
            }
        }

        public static void ModifyLightIntensity(float intensity, Player play)
        {
            if (Warhead.IsInProgress)
            {
                play.Broadcast((ushort)2f, "Nuke is active!");
                return;
            }

            if (!CanTurnOffLights && intensity == 0)
            {
                play.Broadcast((ushort)2f, AmongSCP.Singleton.Config.CoolDownNoCountMessage);
                return;
            }

            if (meetingStarted)
            {
                play.Broadcast((ushort)2f, "Nuke is already active!");
                return;
            }

            foreach (Room room in Exiled.API.Features.Map.Rooms)
            {
                room.SetLightIntensity(intensity);
                curLightIntensity = intensity;
            }
            
            foreach(Player ply in EventHandlers.PlayerManager.AlivePlayers)
            {
                if(ply.GetInfo().Role == PlayerManager.Role.Imposter && intensity == 0)
                {
                    ply.Broadcast((ushort)5f, AmongSCP.Singleton.Config.LightsOffMessage);
                }
                else if(intensity == 0)
                {
                    ply.Broadcast((ushort)5f, AmongSCP.Singleton.Config.FixLightsMessage);
                }
            }
            if(intensity == 0) CanTurnOffLights = false;
            if(intensity == 1) Timing.CallDelayed(AmongSCP.Singleton.Config.LightsCooldown, () => CanTurnOffLights = true);
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
                target.NoClipEnabled = false;
                target.IsInvisible = false;
            }
        }

        public static void RunDetonateWarhead(Player ply)
        {
            if (Warhead.IsInProgress)
            {
                ply.Broadcast((ushort)2f, AmongSCP.Singleton.Config.NukeActiveMessage);
                return;
            }

            if (curLightIntensity != 1)
            {
                ply.Broadcast((ushort)2f, AmongSCP.Singleton.Config.LightsOffMessage);
                return;
            }

            if (meetingStarted)
            {
               ply.Broadcast((ushort)2f, "You are in a meeting!");
               return;
            }

            if (!Util.CanNuke)
            {
                ply.Broadcast((ushort)2f, "You are in a meeting!");
                return;
            }

            Timing.RunCoroutine(DetonateWarhead());
        }

        public static IEnumerator<float> DetonateWarhead()
        {
            Warhead.DetonationTimer = 90;
            Warhead.Start();
            Warhead.LeverStatus = true;

            while(Warhead.IsInProgress)
            {
                if(Warhead.DetonationTimer <= 9.4f)
                {
                    foreach(var ply in EventHandlers.PlayerManager.Imposters)
                    {
                        ply.Position = Exiled.API.Extensions.Role.GetRandomSpawnPoint(RoleType.ChaosInsurgency);
                    }
                    break;
                }
                yield return Timing.WaitForOneFrame;
            }
            CanNuke = false;
            Timing.CallDelayed(AmongSCP.Singleton.Config.NukeCooldown, () => CanNuke = true);
        }

        public static void GhostMode(Player player)
        {
            /*
            foreach(Player ply in Player.List)
            {
                if (!ply.GetInfo().IsAlive)
                {
                    try
                    {
                        MirrorExtensions.SendFakeSyncVar(ply, player.ReferenceHub.networkIdentity, typeof(PlayerEffectsController), nameof(PlayerEffectsController.EnableEffect), EffectType.Scp268);
                    }
                    catch (Exception e)
                    {
                        Log.Debug(e);
                    }
                }     
            }
            */
            player.IsInvisible = true;
            player.Inventory.AddNewItem(ItemType.Adrenaline);
            player.NoClipEnabled = true;
            if (player.GetInfo().Role == PlayerManager.Role.Imposter)
            {
                player.Broadcast((ushort)10f, "You are a ghost!" +
                                               "\n You can noclip and still sabatoge" +
                                               "\n If you get lost, try dropping the adrenaline.");
            }
            else
            {
                player.Broadcast((ushort)10f, "You are a ghost!" +
                               "\n You can noclip and still complete tasks" +
                               "\n If you get lost, try dropping the adrenaline.");
            }
        }
        
        public static IEnumerator<float> Levitate(Pickup pickup, float heightMultiplier, float speedMultiplier)
        {
            var transform = pickup.transform;

            while (pickup != null)
            {
                var vector = transform.position;
                vector.y += heightMultiplier * (float)Math.Sin(Time.timeSinceLevelLoad / speedMultiplier);

                transform.position = vector;
                pickup.Networkposition = vector;

                yield return Timing.WaitForOneFrame;
            }
        }
    }
}