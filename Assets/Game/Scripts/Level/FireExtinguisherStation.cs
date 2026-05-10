using Game.Interaction;
using Game.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Level
{
    public class FireExtinguisherStation : MonoBehaviour, IInteractable
    {
        [Header("Extinguisher")]
        [SerializeField, Tooltip("Pickup prefab used as the extinguisher item.")]
        private PickupItem _extinguisherPrefab;

        [SerializeField, Tooltip("Where the extinguisher appears when available.")]
        private Transform _spawnPoint;

        [SerializeField, Tooltip("Item type assigned to spawned extinguishers.")]
        private ItemType _extinguisherItemType = ItemType.FireExtinguisher;

        [Header("Respawn")]
        [SerializeField, Tooltip("Seconds before a destroyed extinguisher respawns.")]
        private float _respawnDelay = 5f;

        [SerializeField, Tooltip("Optional fill image showing respawn progress from 0 to 1.")]
        private Image _respawnFillImage;

        private PickupItem _activeExtinguisher;
        private float _respawnTimer;
        private bool _isRespawning;

        private void Start()
        {
            SpawnExtinguisher();
            UpdateRespawnFill(0f, false);
        }

        private void Update()
        {
            if (!_isRespawning)
            {
                return;
            }

            _respawnTimer += Time.deltaTime;

            float progress = _respawnDelay > 0f ? Mathf.Clamp01(_respawnTimer / _respawnDelay) : 1f;
            UpdateRespawnFill(progress, true);

            if (progress >= 1f)
            {
                _isRespawning = false;
                _respawnTimer = 0f;
                SpawnExtinguisher();
                UpdateRespawnFill(0f, false);
            }
        }

        public void SetInteractActive(bool isActive)
        {
        }

        public void Interact(PlayerInteractor interactor)
        {
            if (interactor == null || interactor.ItemHolder == null || interactor.ItemHolder.HasItem)
            {
                return;
            }

            if (_activeExtinguisher == null || _isRespawning)
            {
                return;
            }

            PickupItem extinguisher = _activeExtinguisher;
            extinguisher.transform.SetParent(null, true);

            if (!interactor.ItemHolder.PickItem(extinguisher))
            {
                ReturnExtinguisherToSpawn(extinguisher);
            }
        }

        public void NotifyExtinguisherDestroyed(PickupItem extinguisher)
        {
            if (extinguisher == null || extinguisher != _activeExtinguisher)
            {
                return;
            }

            _activeExtinguisher = null;
            BeginRespawn();
        }

        private void SpawnExtinguisher()
        {
            if (_activeExtinguisher != null || _extinguisherPrefab == null)
            {
                return;
            }

            Transform spawnPoint = _spawnPoint != null ? _spawnPoint : transform;
            _activeExtinguisher = Instantiate(_extinguisherPrefab, spawnPoint.position, spawnPoint.rotation);
            _activeExtinguisher.transform.SetParent(spawnPoint);
            _activeExtinguisher.transform.localPosition = Vector3.zero;
            _activeExtinguisher.transform.localRotation = Quaternion.identity;
            _activeExtinguisher.Setup(_extinguisherItemType);
            SetExtinguisherPhysics(_activeExtinguisher, false);

            FireExtinguisherPickupTracker tracker = _activeExtinguisher.GetComponent<FireExtinguisherPickupTracker>();

            if (tracker == null)
            {
                tracker = _activeExtinguisher.gameObject.AddComponent<FireExtinguisherPickupTracker>();
            }

            tracker.Setup(this, _activeExtinguisher);
        }

        private void BeginRespawn()
        {
            _isRespawning = true;
            _respawnTimer = 0f;
            UpdateRespawnFill(0f, true);
        }

        private void UpdateRespawnFill(float progress, bool isVisible)
        {
            if (_respawnFillImage == null)
            {
                return;
            }

            _respawnFillImage.fillAmount = progress;
            _respawnFillImage.enabled = isVisible;
        }

        private void ReturnExtinguisherToSpawn(PickupItem extinguisher)
        {
            if (extinguisher == null)
            {
                return;
            }

            Transform spawnPoint = _spawnPoint != null ? _spawnPoint : transform;
            extinguisher.transform.SetParent(spawnPoint);
            extinguisher.transform.localPosition = Vector3.zero;
            extinguisher.transform.localRotation = Quaternion.identity;
            SetExtinguisherPhysics(extinguisher, false);
        }

        private static void SetExtinguisherPhysics(PickupItem extinguisher, bool isEnabled)
        {
            if (extinguisher == null || !extinguisher.TryGetComponent(out Rigidbody rigidbody))
            {
                return;
            }

            rigidbody.isKinematic = !isEnabled;
            rigidbody.useGravity = isEnabled;
            rigidbody.detectCollisions = isEnabled;
        }
    }
}
