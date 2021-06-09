using System;
using System.Collections.Generic;
using Exiled.API.Features;
using UnityEngine;

namespace AmongSCP
{
    public static class PointManager
    {
        //Seed 1378514975
        public static List<Vector3> PlayerSpawns = new List<Vector3>()
        {
            new Vector3(79.77f, -1005.87f, 40.07f)
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