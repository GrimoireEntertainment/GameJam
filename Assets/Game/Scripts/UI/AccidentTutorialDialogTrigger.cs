using System.Collections;
using Game.Accidents;
using UnityEngine;

namespace Game.UI
{
    public class AccidentTutorialDialogTrigger : MonoBehaviour
    {
        private enum TriggerMode
        {
            AccidentStarted,
            AccidentResolved,
            Time
        }

        [Header("References")]
        [SerializeField, Tooltip("Accident controller that raises accident events.")]
        private AccidentController _accidentController;

        [SerializeField, Tooltip("Dialog controller that displays the tutorial text.")]
        private TutorialDialogController _dialogController;

        [Header("Trigger")]
        [SerializeField, Tooltip("How this tutorial is triggered.")]
        private TriggerMode _triggerMode = TriggerMode.AccidentStarted;

        [SerializeField, Tooltip("Accident type id that should trigger this tutorial, for example Fire or HullBreach.")]
        private string _accidentId;

        [SerializeField, Tooltip("Level time in seconds when this tutorial should appear in Time mode.")]
        private float _triggerTime;

        [SerializeField, TextArea, Tooltip("Tutorial text shown the first time this accident appears.")]
        private string _message;

        [SerializeField, Tooltip("Delay after trigger before showing the dialog.")]
        private float _showDelay = 1.5f;

        [SerializeField, Tooltip("Optional auto-hide duration. 0 means stay visible.")]
        private float _visibleDuration;

        [Header("Position")]
        [SerializeField, Tooltip("Anchored UI position used when this tutorial is shown.")]
        private Vector2 _dialogPosition;

        [Header("One Shot")]
        [SerializeField, Tooltip("Show this tutorial only once per level.")]
        private bool _showOnlyOnce = true;

        private bool _wasShown;
        private Coroutine _showRoutine;

        private void OnEnable()
        {
            if (_triggerMode == TriggerMode.AccidentStarted || _triggerMode == TriggerMode.AccidentResolved)
            {
                if (_accidentController == null)
                {
                    Debug.LogWarning($"{nameof(AccidentTutorialDialogTrigger)} on {name} is missing an accident controller.", this);
                    return;
                }

                if (_triggerMode == TriggerMode.AccidentStarted)
                {
                    _accidentController.AccidentStarted += OnAccidentStarted;
                }
                else
                {
                    _accidentController.AccidentResolved += OnAccidentResolved;
                }
            }
        }

        private void Start()
        {
            if (_triggerMode == TriggerMode.Time)
            {
                TryStartShowRoutine(_triggerTime);
            }
        }

        private void OnDisable()
        {
            if (_accidentController != null)
            {
                _accidentController.AccidentStarted -= OnAccidentStarted;
                _accidentController.AccidentResolved -= OnAccidentResolved;
            }

            if (_showRoutine != null)
            {
                StopCoroutine(_showRoutine);
                _showRoutine = null;
            }
        }

        private void OnAccidentStarted(ActiveAccident accident)
        {
            if (accident == null || accident.TypeId != _accidentId)
            {
                return;
            }

            TryStartShowRoutine(_showDelay);
        }

        private void OnAccidentResolved(ActiveAccident accident)
        {
            if (accident == null || accident.TypeId != _accidentId)
            {
                return;
            }

            TryStartShowRoutine(_showDelay);
        }

        private void TryStartShowRoutine(float delay)
        {
            if (_showOnlyOnce && _wasShown)
            {
                return;
            }

            if (_dialogController == null)
            {
                Debug.LogWarning($"{nameof(AccidentTutorialDialogTrigger)} on {name} is missing a dialog controller.", this);
                return;
            }

            _wasShown = true;

            if (_showRoutine != null)
            {
                StopCoroutine(_showRoutine);
            }

            _showRoutine = StartCoroutine(ShowRoutine(delay));
        }

        private IEnumerator ShowRoutine(float delay)
        {
            if (delay > 0f)
            {
                yield return new WaitForSeconds(delay);
            }

            _showRoutine = null;
            _dialogController.ShowAtAnchoredPosition(_message, _dialogPosition, _visibleDuration);
        }
    }
}
