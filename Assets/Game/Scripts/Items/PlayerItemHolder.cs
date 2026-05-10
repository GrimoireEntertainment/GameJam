using UnityEngine;

namespace Game.Items
{
    public class PlayerItemHolder : MonoBehaviour
    {
        [Header("Holding")]
        [SerializeField, Tooltip("Transform where held items are attached.")]
        private Transform _holdPoint;

        private PickupItem _currentItem;

        [Header("Throwing")]
        [SerializeField, Tooltip("Force applied when throwing an item.")]
        private float _throwForce = 5f;

        public bool HasItem => _currentItem != null;
        public ItemType CurrentItemType => _currentItem != null ? _currentItem.ItemType : ItemType.None;

        public bool PickItem(PickupItem item)
        {
            if (item == null || HasItem)
            {
                return false;
            }

            if (_holdPoint == null)
            {
                Debug.LogWarning($"{nameof(PlayerItemHolder)} on {name} is missing a hold point.", this);
                return false;
            }

            _currentItem = item;

            AttachToHoldPoint(_currentItem);

            return true;
        }

        public void ThrowItem()
        {
            if (_currentItem == null)
            {
                return;
            }

            Rigidbody itemRigidbody = _currentItem.GetComponent<Rigidbody>();

            if (itemRigidbody != null)
            {
                EnablePhysics(_currentItem, true);

                itemRigidbody.AddForce(transform.forward * _throwForce, ForceMode.VelocityChange);
            }

            _currentItem.transform.SetParent(null, true);

            _currentItem = null;
        }

        public bool SpawnItem(ItemType type, ItemPrefabDatabase database)
        {
            if (HasItem || type == ItemType.None)
            {
                return false;
            }

            if (database == null)
            {
                Debug.LogWarning($"{nameof(PlayerItemHolder)} on {name} cannot spawn {type}: database is missing.", this);
                return false;
            }

            PickupItem prefab = database.GetPrefab(type);

            if (prefab == null)
            {
                Debug.LogWarning($"{nameof(ItemPrefabDatabase)} has no prefab for item type {type}.", database);
                return false;
            }

            PickupItem item = Instantiate(prefab);
            item.Setup(type);
            return PickItem(item);
        }

        public void RemoveCurrentItem()
        {
            if (_currentItem == null)
            {
                return;
            }

            EnablePhysics(_currentItem, true);
            _currentItem.transform.SetParent(null, true);
            _currentItem = null;
        }

        public PickupItem TakeCurrentItem()
        {
            if (_currentItem == null)
            {
                return null;
            }

            PickupItem item = _currentItem;
            _currentItem = null;
            EnablePhysics(item, true);
            item.transform.SetParent(null, true);
            return item;
        }

        public void DestroyCurrentItem()
        {
            if (_currentItem == null)
            {
                return;
            }

            PickupItem item = _currentItem;
            _currentItem = null;
            Destroy(item.gameObject);
        }

        public bool TryConsumeCurrentItem(ItemType requiredType)
        {
            if (_currentItem == null || _currentItem.ItemType != requiredType)
            {
                return false;
            }

            DestroyCurrentItem();
            return true;
        }

        private void AttachToHoldPoint(PickupItem item)
        {
            EnablePhysics(item, false);
            item.transform.SetParent(_holdPoint);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
        }

        private static void EnablePhysics(PickupItem item, bool isEnabled)
        {
            if (item == null || !item.TryGetComponent(out Rigidbody rigidbody))
            {
                return;
            }

            rigidbody.isKinematic = !isEnabled;
            rigidbody.useGravity = isEnabled;
            rigidbody.detectCollisions = isEnabled;
        }
    }
}
