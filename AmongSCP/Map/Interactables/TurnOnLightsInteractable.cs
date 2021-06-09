using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Exiled.API.Features;

namespace AmongSCP.Map.Interactables
{
    public class TurnOnLightsInteractable
    {
        Interactable _interactable;

        public TurnOnLightsInteractable(Vector3 position, ItemType item)
        {
            Vector3 pos = position;
            ItemData itemData = new ItemData(item, position, Quaternion.identity, new Vector3(2, 2, 2));
            _interactable = new Interactable(itemData, player => Util.ModifyLightIntensity(1));
        }
    }
}
