namespace AmongSCP
{
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    
    public class EventHandlers
    {
        private readonly AmongSCP _plugin;

        public EventHandlers(AmongSCP plugin)
        {
            _plugin = plugin;
        }

        public void OnGameStart()
        {
            foreach(var ply in Player.List)
            {
                MakePlayerImposter(ply);
                break;
            }
        }

        private static void MakePlayerImposter(Player ply)
        {
            ply.SetRole(RoleType.Scp049);
            ply.ChangeAppearance(RoleType.NtfLieutenant);
        }
    }
}