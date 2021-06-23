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

        private bool _destroyOnInteract = false;
        private bool _pickupOnInteract = false;

        public Interactable(ItemData data, Action<Player> onInteract, bool destroyOnInteract = false, bool pickupOnInteract = false)
        {
            _action = onInteract;
            _destroyOnInteract = destroyOnInteract;
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

            //Timing.RunCoroutine(Levitate(pickup));
        }

        public bool OnInteract(Player p)
        {
            _action(p);
            Log.Debug("Method OnInteract() Invoked.");
            if (_destroyOnInteract)
            {
                Log.Debug("Object Destroyed?");
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

        /*
        public static IEnumerator<float> Levitate(Pickup gameObject)
        {
            while(true)
            {
                gameObject.transform.Translate(Vector3.up * Time.deltaTime, Space.World);
                yield return Timing.WaitForOneFrame;
                gameObject.transform.Translate(Vector3.down * Time.deltaTime, Space.World);
            }
        }
        */
    }
}