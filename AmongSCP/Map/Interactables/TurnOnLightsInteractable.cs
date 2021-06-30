using UnityEngine;

namespace AmongSCP.Map.Interactables
{
    public class TurnOnLightsInteractable
    {
        Interactable _interactable;

        public TurnOnLightsInteractable(Vector3 position, ItemType item)
        {
            var pos = position;
            var itemData = new ItemData(item, position, Quaternion.identity, new Vector3(2, 2, 2));
            _interactable = new Interactable(itemData, player => Util.ModifyLightIntensity(1));
        }
    }
}
