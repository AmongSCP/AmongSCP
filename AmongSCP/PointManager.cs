using System;
using System.Collections.Generic;
using AmongSCP.Map;
using Exiled.API.Enums;
using Exiled.API.Features;
using UnityEngine;

namespace AmongSCP
{
    public static class PointManager
    {
        public static Dictionary<MapPosition, MapPosition> Vents = new Dictionary<MapPosition, MapPosition>()
        {
            {new MapPosition(-2f, 1.3f, .2f, RoomType.Hcz096), new MapPosition(30.2f, 1.2f, -6.6f, RoomType.Hcz106)},//096 to 106
            {new MapPosition(30.7f, 1.2f, -20f, RoomType.Hcz106), new MapPosition(1.4f, 1.3f, -.1f, RoomType.HczChkpB) },//106 to elevB
            {new MapPosition(-7.3f, 1.3f, -.2f, RoomType.HczChkpB), new MapPosition(.6f, 1.3f, .3f, RoomType.HczChkpA) },//elevB to elevA
            {new MapPosition(-7.6f, 1.3f, .2f, RoomType.HczChkpA), new MapPosition(-.6f, -5.9f, -21.1f, RoomType.Hcz079) },//elevA to 079
            {new MapPosition(17.1f, -3.3f, -.1f, RoomType.Hcz079), new MapPosition(-5.5f, 1.3f, -4.2f, RoomType.HczEzCheckpoint) },//079 to Checkpoint
            {new MapPosition(-6.1f, 1.3f, 7.3f, RoomType.HczEzCheckpoint), new MapPosition(-2.1f, -6.6f, 4.6f, RoomType.HczServers) },//Checkpoint to servers
            {new MapPosition(-7.6f, 1.3f, 4.1f, RoomType.HczServers), new MapPosition(-8.5f, 1.3f, .1f, RoomType.Hcz096) },//servers to 096
        };

        public static MapPosition LightsSpawn = new MapPosition(0f, 1.3f, -9.1f, RoomType.HczHid);

        //Seed 1378514975
        public static List<MapPosition> PlayerSpawns = new List<MapPosition>()
        {
            new MapPosition(-2f, -5.9f, -7.7f, RoomType.Hcz079)
        };

        public static Dictionary<Type, MapPosition> InteractableSpawns = new Dictionary<Type, MapPosition>()
        {

        };

        public static void SpawnPlayers(Player[] players)
        {
            for (var i = 0; i < players.Length; i++)
            {
                players[i].Position = PlayerSpawns[i % PlayerSpawns.Count].GetRealPosition();
            }
        }
    }
}