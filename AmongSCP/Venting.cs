namespace AmongSCP
{
    using MEC;
    using Exiled.API.Features;

    public class Venting
    {
        public static void PlayerVent(Player ply)
        {
            ply.IsInvisible = true;
            EventHandlers.ImposterCanKill = false;
            Timing.CallDelayed(AmongSCP.Singleton.Config.VentTime, () =>
            {
                ply.IsInvisible = false;
                Timing.CallDelayed(AmongSCP.Singleton.Config.VentKillCooldown, () => EventHandlers.ImposterCanKill = true);
            });
        }
    }
}
