using System;
using System.Collections.Generic;
using Exiled.API.Features;
using UnityEngine;

namespace AmongSCP
{
    public static class PointManager
    {
        public static Dictionary<Vector3, Vector3> Vents = new Dictionary<Vector3, Vector3>()
        {
            {new Vector3(77.2f, -998.67f, 199.3183f), new Vector3(153.8845f, -998.672f, 199.3856f)},//096 to 106
            {new Vector3(153.8845f, -998.672f, 199.3856f), new Vector3(131.0797f, -998.67f, 116.5661f) },//106 to elevB
            {new Vector3(131.0797f, -998.67f, 116.5661f), new Vector3(129.7677f, -998.67f, 75.77992f) },//elevB to elevA
            {new Vector3(129.7677f, -998.67f, 75.77992f), new Vector3(76.71812f, -1003.175f, 17.77227f) },//elevA to 079
            {new Vector3(76.71812f, -1003.175f, 17.77227f), new Vector3(73.29224f, -998.67f, 75.03807f) },//079 to Checkpoint
            {new Vector3(73.29224f, -998.67f, 75.03807f), new Vector3(71.74154f, -998.6702f, 144.5681f) },//Checkpoint to servers
            {new Vector3(71.74154f, -998.6702f, 144.5681f), new Vector3(77.2f, -998.67f, 199.3183f) },//servers to 096
        };

        public static Vector3 LightsSpawn = new Vector3(80.66904f, -998.67f, 179.4948f);

        //Seed 1378514975
        public static List<Vector3> PlayerSpawns = new List<Vector3>()
        {
            new Vector3(79.70f, -1005.883f, 43.058f) //Inside 079 chamber
        };

        public static Dictionary<Type, Vector3> InteractableSpawns = new Dictionary<Type, Vector3>()
        {

        };

        public static void SpawnPlayers(Player[] players)
        {
            for (var i = 0; i < players.Length; i++)
            {
                players[i].Position = PlayerSpawns[i % PlayerSpawns.Count];
            }
        }
    }
}