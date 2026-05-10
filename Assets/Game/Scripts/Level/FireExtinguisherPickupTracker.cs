using Game.Items;
using UnityEngine;

namespace Game.Level
{
    public class FireExtinguisherPickupTracker : MonoBehaviour
    {
        private FireExtinguisherStation _station;
        private PickupItem _pickupItem;

        public void Setup(FireExtinguisherStation station, PickupItem pickupItem)
        {
            _station = station;
            _pickupItem = pickupItem;
        }

        private void OnDestroy()
        {
            if (_station != null)
            {
                _station.NotifyExtinguisherDestroyed(_pickupItem);
            }
        }
    }
}
