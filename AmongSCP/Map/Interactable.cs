using System;
using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Mirror;
using UnityEngine;
using Object = UnityEngine.Object;
using MEC;

namespace AmongSCP.Map
{
    public class Interactable
    {
        public Pickup Pickup;
        private InteractableBehavior _interactable;
        private Action<Player> _action;

        private bool _destroyOnInteract;
        private bool _pickupOnInteract;
//ss
        public Interactable(ItemData data, Action<Player> onInteract, bool destroyOnInteract = false, bool pickupOnInteract = false, bool levitate = false, float levitateHeight = 1.5f, float levitateSpeed = 1)
        {
            _action = onInteract;
            _destroyOnInteract = destroyOnInteract;
            _pickupOnInteract = pickupOnInteract;

            var itemObj = new Item(Server.Host.Inventory.CreateItemInstance(data.item, false)) {Scale = data.scale};

            var pickup = itemObj.Spawn(data.pos.GetRealPosition(), data.rot);

            if (!data.hasPhysics)
            {
                var rb = pickup.Base.gameObject.GetComponent<Rigidbody>();
                rb.useGravity = false;
                rb.isKinematic = true;
            }

            Pickup = pickup;
            _interactable = pickup.Base.gameObject.AddComponent<InteractableBehavior>();
            _interactable.Interactable = this;

           //if(levitate) Timing.RunCoroutine(Util.Levitate(pickup, levitateHeight, levitateSpeed));
        }

        public bool OnInteract(Player p)
        {
            _action(p);
            if (_destroyOnInteract)
            {
                _interactable.Interactable = null;
                
                Object.Destroy(Pickup.Base.gameObject);

                return false;
            }

            if (!_pickupOnInteract) return false;

            return true;
        }
    }
}