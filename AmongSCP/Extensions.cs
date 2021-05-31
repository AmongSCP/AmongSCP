using System;
using AmongSCP.PlayerManager;
using Exiled.API.Features;

namespace AmongSCP
{
    public static class Extensions
    {
        public static PlayerInfo GetInfo(this Player p)
        {
            if (EventHandlers.PlayerManager.Players.TryGetValue(p, out var info)) return info;
            
            EventHandlers.PlayerManager.UpdateQueueNoWait();
            
            if (EventHandlers.PlayerManager.Players.TryGetValue(p, out var info1)) return info1;

            throw new Exception("The player does not exist!");
        }
    }
}