using System.Collections.Generic;
using Game.Interaction;
using Game.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Crafting
{
    public class CraftStation : MonoBehaviour, IInteractable, IHoldInteractable
    {
        [Header("Slot")]
        [SerializeField]
        private Transform _slotPoint;

        [Header("Items")]
        [SerializeField]
        private ItemPrefabDatabase _itemPrefabDatabase;

        [Header("Hold")]
        [SerializeField]
        private float _craftHoldDuration = 1.5f;

        [SerializeField]
        private Slider _holdProgressSlider;

        [Header("Recipes")]
        [SerializeField]
        private List<CraftRecipe> _recipes = new()
        {
            new CraftRecipe
            {
                First = ItemType.Iron,
                Second = ItemType.Iron,
                Result = ItemType.Gear
            },

            new CraftRecipe
            {
                First = ItemType.Iron,
                Second = ItemType.Chemistry,
                Result = ItemType.Battery
            },

            new CraftRecipe
            {
                First = ItemType.Battery,
                Second = ItemType.Iron,
                Result = ItemType.MagneticPlug
            }
        };

        private PickupItem _slotItem;

        public float HoldDuration => _craftHoldDuration;

        private void Awake()
        {
            HideSlider();
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

            PlayerItemHolder itemHolder = interactor.ItemHolder;

            // Put first item instantly
            if (itemHolder.HasItem && _slotItem == null)
            {
                PlaceInSlot(itemHolder.TakeCurrentItem());
                return;
            }

            // Take item instantly
            if (!itemHolder.HasItem && _slotItem != null)
            {
                PickupItem item = TakeSlotItem();

                itemHolder.PickItem(item);

                return;
            }

            // Swap instantly if recipe does not exist
            if (itemHolder.HasItem && _slotItem != null)
            {
                CraftRecipe recipe =
                    GetMatchingRecipe(_slotItem.ItemType, itemHolder.CurrentItemType);

                if (recipe == null)
                {
                    SwapWithHolder(itemHolder);
                }
            }
        }

        public bool CanHoldInteract(PlayerInteractor interactor)
        {
            if (interactor == null || interactor.ItemHolder == null)
            {
                return false;
            }

            if (_slotItem == null)
            {
                return false;
            }

            if (!interactor.ItemHolder.HasItem)
            {
                return false;
            }

            return GetMatchingRecipe(
                _slotItem.ItemType,
                interactor.ItemHolder.CurrentItemType) != null;
        }

        public void StartHold(PlayerInteractor interactor)
        {
            ShowSlider();
            SetSliderProgress(0f);
        }

        public void ProcessHold(PlayerInteractor interactor, float progress)
        {
            SetSliderProgress(progress);
        }

        public void CompleteHold(PlayerInteractor interactor)
        {
            HideSlider();

            if (interactor == null || interactor.ItemHolder == null)
            {
                return;
            }

            if (_slotItem == null)
            {
                return;
            }

            PlayerItemHolder itemHolder = interactor.ItemHolder;

            if (!itemHolder.HasItem)
            {
                return;
            }

            CraftRecipe recipe =
                GetMatchingRecipe(_slotItem.ItemType, itemHolder.CurrentItemType);

            if (recipe == null)
            {
                return;
            }

            Craft(itemHolder, recipe.Result);
        }

        public void CancelHold(PlayerInteractor interactor)
        {
            HideSlider();
        }

        private void ShowSlider()
        {
            if (_holdProgressSlider == null)
            {
                return;
            }

            _holdProgressSlider.gameObject.SetActive(true);
        }

        private void HideSlider()
        {
            if (_holdProgressSlider == null)
            {
                return;
            }

            _holdProgressSlider.gameObject.SetActive(false);
            _holdProgressSlider.value = 0f;
        }

        private void SetSliderProgress(float progress)
        {
            if (_holdProgressSlider == null)
            {
                return;
            }

            _holdProgressSlider.value = progress;
        }

        private void Craft(PlayerItemHolder itemHolder, ItemType resultType)
        {
            if (!CanSpawnResult(resultType))
            {
                return;
            }

            PickupItem slotItem = TakeSlotItem();

            Destroy(slotItem.gameObject);

            itemHolder.DestroyCurrentItem();

            itemHolder.SpawnItem(
                resultType,
                _itemPrefabDatabase);
        }

        private void SwapWithHolder(PlayerItemHolder itemHolder)
        {
            PickupItem stationItem = TakeSlotItem();
            PickupItem playerItem = itemHolder.TakeCurrentItem();

            PlaceInSlot(playerItem);

            itemHolder.PickItem(stationItem);
        }

        private void PlaceInSlot(PickupItem item)
        {
            if (item == null)
            {
                return;
            }

            _slotItem = item;

            SetItemPhysics(_slotItem, false);

            _slotItem.transform.SetParent(_slotPoint);
            _slotItem.transform.localPosition = Vector3.zero;
            _slotItem.transform.localRotation = Quaternion.identity;
        }

        private PickupItem TakeSlotItem()
        {
            PickupItem item = _slotItem;

            _slotItem = null;

            if (item == null)
            {
                return null;
            }

            item.transform.SetParent(null, true);

            SetItemPhysics(item, true);

            return item;
        }

        private CraftRecipe GetMatchingRecipe(ItemType a, ItemType b)
        {
            for (int i = 0; i < _recipes.Count; i++)
            {
                CraftRecipe recipe = _recipes[i];

                if (recipe != null && recipe.Matches(a, b))
                {
                    return recipe;
                }
            }

            return null;
        }

        private bool CanSpawnResult(ItemType resultType)
        {
            if (_itemPrefabDatabase == null)
            {
                return false;
            }

            return _itemPrefabDatabase.GetPrefab(resultType) != null;
        }

        private static void SetItemPhysics(PickupItem item, bool isEnabled)
        {
            if (item == null || !item.TryGetComponent(out Rigidbody rigidbody))
            {
                return;
            }

            rigidbody.isKinematic = !isEnabled;
            rigidbody.detectCollisions = isEnabled;
        }
    }
}