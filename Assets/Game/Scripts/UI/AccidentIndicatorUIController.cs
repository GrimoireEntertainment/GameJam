using System.Collections.Generic;
using Game.Accidents;
using UnityEngine;

namespace Game.UI
{
    public class AccidentIndicatorUIController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Tooltip("Accident controller that raises UI events.")]
        private AccidentController _accidentController;

        [SerializeField, Tooltip("Prefab used for one active accident indicator.")]
        private AccidentIndicatorUIItem _indicatorPrefab;

        [SerializeField, Tooltip("Parent transform for spawned accident indicators.")]
        private Transform _container;

        private readonly Dictionary<string, AccidentIndicatorUIItem> _itemsByAccidentId = new();

        private void OnEnable()
        {
            if (_accidentController == null)
            {
                Debug.LogWarning($"{nameof(AccidentIndicatorUIController)} on {name} is missing an accident controller.", this);
                return;
            }

            _accidentController.AccidentStarted += OnAccidentStarted;
            _accidentController.AccidentResolved += OnAccidentResolved;
        }

        private void OnDisable()
        {
            if (_accidentController != null)
            {
                _accidentController.AccidentStarted -= OnAccidentStarted;
                _accidentController.AccidentResolved -= OnAccidentResolved;
            }
        }

        private void OnAccidentStarted(ActiveAccident accident)
        {
            if (accident == null || accident.Definition == null || string.IsNullOrWhiteSpace(accident.InstanceId))
            {
                return;
            }

            if (_itemsByAccidentId.ContainsKey(accident.InstanceId))
            {
                return;
            }

            if (_indicatorPrefab == null || _container == null)
            {
                Debug.LogWarning($"{nameof(AccidentIndicatorUIController)} on {name} cannot create an indicator: prefab or container is missing.", this);
                return;
            }

            AccidentIndicatorUIItem item = Instantiate(_indicatorPrefab, _container);
            item.Setup(accident.Definition);
            _itemsByAccidentId.Add(accident.InstanceId, item);
        }

        private void OnAccidentResolved(ActiveAccident accident)
        {
            if (accident == null || string.IsNullOrWhiteSpace(accident.InstanceId))
            {
                return;
            }

            if (!_itemsByAccidentId.TryGetValue(accident.InstanceId, out AccidentIndicatorUIItem item))
            {
                return;
            }

            _itemsByAccidentId.Remove(accident.InstanceId);

            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }
    }
}
