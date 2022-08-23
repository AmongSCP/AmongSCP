using System;
using System.Collections.Generic;
using AmongSCP.Map.Interactables;
using Exiled.API.Features;
using MEC;
using Exiled.API.Extensions;
using Interactables.Interobjects.DoorUtils;
using UnityEngine;
using Exiled.API.Enums;
using Mirror;

namespace AmongSCP
{
    public static class Util
    {
        public static int VoteAmount = 0;

        public static bool meetingStarted = false;

        public static bool CanTurnOffLights = true;

        public static bool CanNuke = true;

        public static float curLightIntensity = 1;

        public static bool globalCanCallEmergencyMeeting = true;

        public static bool initialCooldownOn = true;

        public static IEnumerator<float> CallEmergencyMeeting(Player caller, string message, bool isBodyReport)
        {
            Warhead.Stop();
            meetingStarted = true;
            var meetingTime = isBodyReport ? AmongSCP.Singleton.Config.BodyReportedMeetingTime : AmongSCP.Singleton.Config.EmergencyTime;

            var votePos = AmongSCP.Singleton.Config.VotePosition;

            Exiled.API.Features.Map.Broadcast((ushort)5f, message + "\n Interact with a persons hat to vote for them or drop your flashlight to skip!");

            foreach (var ply in Player.List)
            {
                if (ply.GetInfo().IsAlive)
                {
                    ply.Position = votePos;
                    ply.EnableEffect<MeetingEffect>(meetingTime);
                }
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

                if (meetingTime >= 1 && VoteAmount != EventHandlers.PlayerManager.AlivePlayers.Count) continue;

                meetingStarted = false;
                PointManager.SpawnPlayers(EventHandlers.PlayerManager.AlivePlayers.ToArray());
                VoteAmount = 0;
                caller.GetInfo().CalledEmergencyMeeting = false;

                EventHandlers.PlayerManager.KillMostVotedPlayer();
                EventHandlers.PlayerManager.ClearAllPlayersVotes();

                globalCanCallEmergencyMeeting = false;
                Timing.CallDelayed(AmongSCP.Singleton.Config.globalEmergencyMeetingCooldown, () => globalCanCallEmergencyMeeting = false);
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
                play.Broadcast((ushort)2f, "You are on cooldown!");
                return;
            }

            if (meetingStarted)
            {
                play.Broadcast((ushort)2f, "Nuke is already active!");
                return;
            }

            foreach (Room room in Exiled.API.Features.Room.List)
            {
                room.LightIntensity = intensity;
                curLightIntensity = intensity;
            }

            foreach (Player ply in EventHandlers.PlayerManager.AlivePlayers)
            {
                if (ply.GetInfo().Role == PlayerManager.Role.Imposter && intensity == 0)
                {
                    ply.Broadcast((ushort)5f, "Lights are off!");
                }
                else if (intensity == 0)
                {
                    ply.Broadcast((ushort)5f, "Fix Lights in Micro!");
                }
            }
            if (intensity == 0) CanTurnOffLights = false;
            if (intensity == 1) Timing.CallDelayed(AmongSCP.Singleton.Config.LightsCooldown, () => CanTurnOffLights = true);
        }

        public static void SetUpDoors()
        {
            foreach (var door in Exiled.API.Features.Door.List)
            {
                door.ChangeLock(DoorLockType.SpecialDoorFeature);
                door.IsOpen = true;
            }
        }

        public static void ChangeOutfit(NetworkIdentity ply, RoleType role)
        {
            foreach (var target in Player.List)
            {
                //If the target is not an imposter, show the fake role, otherwise show the true role.
                if (!EventHandlers.PlayerManager.Imposters.Contains(target))
                {
                    MirrorExtensions.SendFakeSyncVar(target, ply, typeof(CharacterClassManager), nameof(CharacterClassManager.NetworkCurClass), (sbyte)AmongSCP.Singleton.Config.CrewmateRole);
                }
                else
                {
                    MirrorExtensions.SendFakeSyncVar(target, ply, typeof(CharacterClassManager), nameof(CharacterClassManager.NetworkCurClass), (sbyte)role);
                }
                target.NoClipEnabled = false;
                target.IsInvisible = false;
            }
        }

        public static void RunDetonateWarhead(Player ply)
        {
            if (Warhead.IsInProgress)
            {
                ply.Broadcast((ushort)2f, "Nuke is already active!");
                return;
            }

            if (curLightIntensity != 1)
            {
                ply.Broadcast((ushort)2f, "Lights are off!");
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
            try
            {
                Log.Debug("DetonateWarhead invoked.", AmongSCP.Singleton.Config.showLogs);
            }
            catch (Exception e)
            {
                Log.Debug(e, AmongSCP.Singleton.Config.showLogs);
            }
            Warhead.DetonationTimer = 90;
            Warhead.Start();
            Warhead.LeverStatus = true;

            while (Warhead.IsInProgress)
            {
                if (Warhead.DetonationTimer <= 9.4f)
                {
                    foreach (var ply in EventHandlers.PlayerManager.Imposters)
                    {
                        ply.Position = RoleType.ChaosConscript.GetRandomSpawnProperties().Item1;
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
            player.IsInvisible = true;
            player.AddItem(ItemType.Adrenaline);
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

        /*public static IEnumerator<float> Levitate(Pickup pickup, float heightMultiplier, float speedMultiplier)
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
        }*/
    }
}