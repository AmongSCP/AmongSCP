using UnityEngine;

namespace AmongSCP.Map
{
    public struct ItemData
    {
        public ItemType item;
        public Vector3 pos;
        public Quaternion rot;
        public Vector3 scale;
        public bool hasPhysics;
        
        public ItemData(ItemType item, Vector3 pos, Quaternion rot, Vector3 scale = default, bool hasPhysics = false)
        {
            this.item = item;
            this.pos = pos;
            this.rot = rot;
            this.scale = scale == default ? Vector3.one : scale;
            this.hasPhysics = hasPhysics;
        }
    }
}