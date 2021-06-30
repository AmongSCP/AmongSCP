using System;
using System.Collections.Generic;
using Exiled.API.Features;
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

        public Interactable(ItemData data, Action<Player> onInteract, bool destroyOnInteract = false, bool pickupOnInteract = false, bool levitate = false, float levitateHeight = 1.5f, float levitateSpeed = 1)
        {
            _action = onInteract;
            _destroyOnInteract = destroyOnInteract;
            _pickupOnInteract = pickupOnInteract;

            var gameObject = UnityEngine.Object.Instantiate<GameObject>(Server.Host.Inventory.pickupPrefab);

            gameObject.transform.localScale = data.scale;

            if (!data.hasPhysics)
            {
                var rb = gameObject.GetComponent<Rigidbody>();
                rb.useGravity = false;
                rb.isKinematic = true;

                var collider = gameObject.GetComponent<Collider>();
                collider.enabled = false;
            }
            
            NetworkServer.Spawn(gameObject);
            
            var pickup = gameObject.GetComponent<Pickup>();
            pickup.SetupPickup(data.item, 0, Server.Host.Inventory.gameObject, new Pickup.WeaponModifiers(true, 0, 0, 0), data.pos, data.rot);

            Pickup = pickup;
            _interactable = gameObject.AddComponent<InteractableBehavior>();
            _interactable.Interactable = this;

           if(levitate) Timing.RunCoroutine(Util.Levitate(pickup, levitateHeight, levitateSpeed));
        }

        public bool OnInteract(Player p)
        {
            _action(p);
            if (_destroyOnInteract)
            {
                _interactable.Interactable = null;
                
                Object.Destroy(Pickup.gameObject);

                return false;
            }

            if (!_pickupOnInteract) return false;

            _interactable.Interactable = null;
            Object.Destroy(_interactable);
            _interactable = null;

            return true;
        }
    }
}