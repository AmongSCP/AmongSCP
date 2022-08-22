using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using UnityEngine;

namespace AmongSCP.Map
{
    public class MapPosition
    {
        public Vector3 Position { get; set; }
        public RoomType RoomType { get; set; }

        public MapPosition(Vector3 position, RoomType roomType)
        {
            Position = position;
            RoomType = roomType;
        }

        public MapPosition(float x, float y, float z, RoomType roomType) : this(new Vector3(x, y, z), roomType)
        {
        }

        public MapPosition(Vector3 position)
        {
            Position = position;
            RoomType = RoomType.Unknown;
        }
        
        public MapPosition(float x, float y, float z) : this(new Vector3(x, y, z))
        {
        }

        public static Vector3 CalculateOffset(Vector3 position, RoomType roomType)
        {
            if (roomType == RoomType.Unknown)
                return position;

            // We need to "normalize" the position by rotating it about the room, using the opposite of the room's rotation.
            var room = Room.List.First(room1 => room1.Type == roomType);
            var diff = position - room.Position;

            return Quaternion.Inverse(room.Transform.rotation) * diff;
        }

        public Vector3 GetRealPosition()
        {
            if (RoomType == RoomType.Unknown)
                return Position;
            
            // We need to "de-normalize" the position by rotating it about the room.
            var room = Room.List.First(room1 => room1.Type == RoomType);
            var normalized = room.Transform.rotation * Position;

            // Then, add it back to the room position.
            return normalized + room.Position;
        }
    }
}