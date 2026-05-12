using Game.Interaction;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Items
{
    public class ResourceBox : MonoBehaviour, IInteractable, IHoldInteractable
    {
        [Header("Resource")]
        [SerializeField]
        private ItemType _resourceType = ItemType.Crystal;

        [SerializeField]
        private ItemPrefabDatabase _itemPrefabDatabase;

        [Header("Hold")]
        [SerializeField]
        private float _holdDuration;

        [SerializeField]
        private Slider _holdProgressSlider;

        public float HoldDuration => _holdDuration;

        private void Awake()
        {
            if (_holdProgressSlider != null) _holdProgressSlider.gameObject.SetActive(false);
        }

        public void SetInteractActive(bool isActive)
        {
        }

        public void Interact(PlayerInteractor interactor)
        {
        }

        public bool CanHoldInteract(PlayerInteractor interactor)
        {
            if (interactor == null || interactor.ItemHolder == null)
            {
                return false;
            }

            return !interactor.ItemHolder.HasItem;
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

            if (interactor.ItemHolder.HasItem)
            {
                return;
            }

            interactor.ItemHolder.SpawnItem(
                _resourceType,
                _itemPrefabDatabase);
        }

        public void CancelHold(PlayerInteractor interactor)
        {
            HideSlider();
        }

        private void ShowSlider()
        {
            if (_holdDuration == 0f || _holdProgressSlider == null)
            {
                return;
            }

            _holdProgressSlider.gameObject.SetActive(true);
        }

        private void HideSlider()
        {
            if (_holdDuration == 0f || _holdProgressSlider == null)
            {
                return;
            }

            _holdProgressSlider.gameObject.SetActive(false);
            _holdProgressSlider.value = 0f;
        }

        private void SetSliderProgress(float progress)
        {
            if (_holdDuration == 0f || _holdProgressSlider == null)
            {
                return;
            }

            _holdProgressSlider.value = progress;
        }
    }
}