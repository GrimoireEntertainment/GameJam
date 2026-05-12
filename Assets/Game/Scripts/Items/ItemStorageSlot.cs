using Game.Audio;
using Game.Core;
using Game.Interaction;
using UnityEngine;

namespace Game.Items
{
    public class ItemStorageSlot : MonoBehaviour, IInteractable
    {
        [Header("Slot")]
        [SerializeField, Tooltip("Point where the stored item is placed.")]
        private Transform _slotPoint;

        [Header("Initial Item")]
        [SerializeField, Tooltip("Optional existing scene item to place in this slot on start.")]
        private PickupItem _initialSceneItem;

        [SerializeField, Tooltip("Optional prefab spawned into this slot on start if no scene item is assigned.")]
        private PickupItem _initialItemPrefab;

        [SerializeField, Tooltip("Item type assigned to spawned initial prefab.")]
        private ItemType _initialItemType = ItemType.None;

        private PickupItem _storedItem;

        public bool HasItem => _storedItem != null;
        public ItemType StoredItemType => _storedItem != null ? _storedItem.ItemType : ItemType.None;

        private void Start()
        {
            SetupInitialItem();
        }

        public void SetInteractActive(bool isActive)
        {
        }

        public void Interact(PlayerInteractor interactor)
        {
            if (interactor == null || interactor.ItemHolder == null)
            {
                return;
            }

            if (_slotPoint == null)
            {
                Debug.LogWarning($"{nameof(ItemStorageSlot)} on {name} is missing a slot point.", this);
                return;
            }

            PlayerItemHolder itemHolder = interactor.ItemHolder;

            if (itemHolder.HasItem && _storedItem == null)
            {
                PlaceItem(itemHolder.TakeCurrentItem());
                return;
            }

            if (!itemHolder.HasItem && _storedItem != null)
            {
                itemHolder.PickItem(TakeItem());
                return;
            }

            if (itemHolder.HasItem && _storedItem != null)
            {
                SwapWithHolder(itemHolder);
            }
        }

        private void SwapWithHolder(PlayerItemHolder itemHolder)
        {
            PickupItem storedItem = TakeItem();
            PickupItem playerItem = itemHolder.TakeCurrentItem();

            PlaceItem(playerItem);
            itemHolder.PickItem(storedItem);
        }

        private void SetupInitialItem()
        {
            if (_storedItem != null || _slotPoint == null)
            {
                return;
            }

            if (_initialSceneItem != null)
            {
                PlaceItem(_initialSceneItem, playAudio: false);
                return;
            }

            if (_initialItemPrefab == null || _initialItemType == ItemType.None)
            {
                return;
            }

            PickupItem item = Instantiate(_initialItemPrefab);
            item.Setup(_initialItemType);
            PlaceItem(item, playAudio: false);
        }

        private void PlaceItem(PickupItem item, bool playAudio = true)
        {
            if (item == null)
            {
                return;
            }

            _storedItem = item;
            SetItemPhysics(_storedItem, false);

            Transform itemTransform = _storedItem.transform;
            itemTransform.SetParent(_slotPoint);
            itemTransform.localPosition = Vector3.zero;
            itemTransform.localRotation = Quaternion.identity;

            if (playAudio)
            {
                AudioService.Instance?.PlaySfx(GameSoundId.ItemPlaced);
            }
        }

        private PickupItem TakeItem()
        {
            PickupItem item = _storedItem;
            _storedItem = null;

            if (item == null)
            {
                return null;
            }

            item.transform.SetParent(null, true);
            SetItemPhysics(item, true);
            return item;
        }

        private static void SetItemPhysics(PickupItem item, bool isEnabled)
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
