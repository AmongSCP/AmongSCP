namespace AmongSCP
{
    using MEC;
    using Exiled.API.Features;
    
    class Venting
    {
        private static void PlayerVent(Player ply)
        {
            ply.IsInvisible = true;
            EventHandlers.ImposterCanKill = false;
            Timing.CallDelayed(AmongSCP.Singleton.Config.VentTime, () =>
            {
                ply.IsInvisible = false;
                Timing.CallDelayed(AmongSCP.Singleton.Config.VentKillCooldown, () => EventHandlers.ImposterCanKill = true);
            });
            
            /*DoorVariant CurDooor = Exiled.API.Features.Map.GetDoorByName(door);
            Vector3 doorPos = CurDooor.transform.position;
            ply.Position = doorPos; */
        }
    }
}
