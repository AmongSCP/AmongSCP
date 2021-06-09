using UnityEngine;

namespace AmongSCP
{
    public class Position
    {
        public float X { get; set; } = 54f;
        public float Y { get; set; } = 1020f;
        public float Z { get; set; } = -45f;

        private Vector3 _vector3 = default;

        public Vector3 GetPositions()
        {
            if (_vector3 == default)
            {
                _vector3 = new Vector3(X, Y, Z);
            }

            return _vector3;
        }
    }
}