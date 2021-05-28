namespace AmongSCP
{
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using System.Collections.Generic;
    using System.Linq;

    public class EventHandlers
    {
        private readonly AmongSCP _plugin;
        private List<Player> _imposters = new List<Player>();  


        public EventHandlers(AmongSCP plugin)
        {
            _plugin = plugin;
        }

        public void OnGameStart()
        {
            
        }

        private void ChangeOutfit(Player ply, RoleType type)
        {
            foreach (var target in Player.List.Where(x => x != ply))
            {
                if(_imposters.Contains(ply))
                {
                    MirrorExtensions.SendFakeSyncVar(target, ply.ReferenceHub.networkIdentity, typeof(CharacterClassManager), nameof(CharacterClassManager.NetworkCurClass), (sbyte)type);
                }
            }
        }
    }
}