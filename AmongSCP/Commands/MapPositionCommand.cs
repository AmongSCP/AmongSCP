using System;
using AmongSCP.Map;
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;

namespace AmongSCP.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class MapPositionCommand : ICommand
    {
        public string Command { get; } = "mapposition";
        public string[] Aliases { get; } = Array.Empty<string>();
        public string Description { get; } = "Returns your current position using room offsets.";
        
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!(sender is PlayerCommandSender p))
            {
                response = "You must be a player to use this command!";
                return false;
            }

            var player = Player.Get(p);
            
            response = "You are in the room " + player.CurrentRoom.Type + " at offset " +
                       MapPosition.CalculateOffset(player.Position, player.CurrentRoom.Type) + ".";
            return true;
        }
    }
}