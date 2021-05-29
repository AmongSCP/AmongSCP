using System;
using System.Collections.Generic;
using Exiled.API.Features;
using UnityEngine;

namespace AmongSCP
{
    public static class PointManager
    {
        public static List<Vector3> PlayerSpawns = new List<Vector3>()
        {

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