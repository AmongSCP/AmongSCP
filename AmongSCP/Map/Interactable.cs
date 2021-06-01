using System;
using Exiled.API.Features;
using Mirror;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AmongSCP.Map
{
    public class Interactable
    {
        private Pickup _pickup;
        private InteractableBehavior _interactable;
        private Action<Player> _action;

        private bool destroyOnInteract = false;
        private bool pickupOnInteract = false;

        public Interactable(ItemData data, Action<Player> onInteract, bool destroyOnInteract = false, bool pickupOnInteract = false)
        {
            _action = onInteract;

            var gameObject = UnityEngine.Object.Instantiate<GameObject>(Server.Host.Inventory.pickupPrefab);

            gameObject.transform.localScale = data.scale;
            
            NetworkServer.Spawn(gameObject);
            
            var pickup = gameObject.GetComponent<Pickup>();
            pickup.SetupPickup(data.item, 0, Server.Host.Inventory.gameObject, new Pickup.WeaponModifiers(true, 0, 0, 0), data.pos, data.rot);

            _pickup = pickup;
            _interactable = gameObject.AddComponent<InteractableBehavior>();
            _interactable.Interactable = this;
        }

        public bool OnInteract(Player p)
        {
            _action(p);

            if (destroyOnInteract)
            {
                _interactable.Interactable = null;
                
                Object.Destroy(_pickup.gameObject);

                return false;
            }

            if (!pickupOnInteract) return false;

            _interactable.Interactable = null;
            Object.Destroy(_interactable);
            _interactable = null;

            return true;
        }
    }
}