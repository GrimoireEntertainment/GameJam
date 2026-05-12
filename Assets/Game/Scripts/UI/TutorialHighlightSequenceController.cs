using System;
using System.Collections.Generic;
using Game.Accidents;
using Game.Interaction;
using UnityEngine;

namespace Game.UI
{
    public class TutorialHighlightSequenceController : MonoBehaviour
    {
        private enum StartMode
        {
            OnEnable,
            Time,
            AccidentStarted,
            AccidentResolved
        }

        [Serializable]
        private class TutorialHighlightStep
        {
            [Tooltip("Interactable object that must be used to complete this step.")]
            public MonoBehaviour TargetInteractable;

            [Tooltip("Optional render root. If empty, Target Interactable is used.")]
            public GameObject RenderRoot;
        }

        private class RendererMaterialsState
        {
            public Renderer Renderer;
            public Material[] Materials;
        }

        [Header("References")]
        [SerializeField, Tooltip("Player interactor that reports completed interactions.")]
        private PlayerInteractor _playerInteractor;

        [SerializeField, Tooltip("Accident controller used by accident start/end triggers.")]
        private AccidentController _accidentController;

        [Header("Highlight")]
        [SerializeField, Tooltip("Material appended to renderers while a tutorial step is active.")]
        private Material _highlightMaterial;

        [SerializeField, Tooltip("Include inactive renderers when searching under the target.")]
        private bool _includeInactiveRenderers;

        [Header("Start")]
        [SerializeField, Tooltip("How this tutorial sequence starts.")]
        private StartMode _startMode = StartMode.OnEnable;

        [SerializeField, Tooltip("Delay before starting when using Time mode.")]
        private float _startDelay;

        [SerializeField, Tooltip("Accident type id used by AccidentStarted or AccidentResolved modes.")]
        private string _accidentId;

        [Header("Steps")]
        [SerializeField, Tooltip("Ordered list of interactable objects to highlight.")]
        private List<TutorialHighlightStep> _steps = new();

        [SerializeField, Tooltip("Do not restart this sequence after it has already started once.")]
        private bool _startOnlyOnce = true;

        private readonly List<RendererMaterialsState> _activeRendererStates = new();
        private int _currentStepIndex = -1;
        private float _timer;
        private bool _isRunning;
        private bool _hasStarted;

        private void OnEnable()
        {
            if (_playerInteractor != null)
            {
                _playerInteractor.Interacted += OnPlayerInteracted;
            }

            if (_accidentController != null)
            {
                _accidentController.AccidentStarted += OnAccidentStarted;
                _accidentController.AccidentResolved += OnAccidentResolved;
            }

            if (_startMode == StartMode.OnEnable)
            {
                StartSequence();
            }
        }

        private void OnDisable()
        {
            ClearHighlight();

            if (_playerInteractor != null)
            {
                _playerInteractor.Interacted -= OnPlayerInteracted;
            }

            if (_accidentController != null)
            {
                _accidentController.AccidentStarted -= OnAccidentStarted;
                _accidentController.AccidentResolved -= OnAccidentResolved;
            }
        }

        private void Update()
        {
            if (_hasStarted || _startMode != StartMode.Time)
            {
                return;
            }

            _timer += Time.deltaTime;

            if (_timer >= _startDelay)
            {
                StartSequence();
            }
        }

        public void StartSequence()
        {
            if (_startOnlyOnce && _hasStarted)
            {
                return;
            }

            if (_steps.Count == 0)
            {
                return;
            }

            _hasStarted = true;
            _isRunning = true;
            _currentStepIndex = 0;
            ApplyCurrentHighlight();
        }

        public void StopSequence()
        {
            _isRunning = false;
            _currentStepIndex = -1;
            ClearHighlight();
        }

        private void OnPlayerInteracted(IInteractable interactable)
        {
            if (!_isRunning || interactable == null || !IsCurrentTarget(interactable))
            {
                return;
            }

            AdvanceStep();
        }

        private void OnAccidentStarted(ActiveAccident accident)
        {
            if (_startMode == StartMode.AccidentStarted && MatchesAccident(accident))
            {
                StartSequence();
            }
        }

        private void OnAccidentResolved(ActiveAccident accident)
        {
            if (_startMode == StartMode.AccidentResolved && MatchesAccident(accident))
            {
                StartSequence();
            }
        }

        private void AdvanceStep()
        {
            ClearHighlight();
            _currentStepIndex++;

            if (_currentStepIndex >= _steps.Count)
            {
                _isRunning = false;
                return;
            }

            ApplyCurrentHighlight();
        }

        private void ApplyCurrentHighlight()
        {
            ClearHighlight();

            if (_highlightMaterial == null || !TryGetCurrentStep(out TutorialHighlightStep step))
            {
                return;
            }

            GameObject renderRoot = step.RenderRoot != null
                ? step.RenderRoot
                : step.TargetInteractable != null
                    ? step.TargetInteractable.gameObject
                    : null;

            if (renderRoot == null)
            {
                return;
            }

            Renderer[] renderers = renderRoot.GetComponentsInChildren<Renderer>(_includeInactiveRenderers);

            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer targetRenderer = renderers[i];

                if (targetRenderer == null)
                {
                    continue;
                }

                Material[] originalMaterials = targetRenderer.materials;
                Material[] highlightedMaterials = new Material[originalMaterials.Length + 1];

                for (int j = 0; j < originalMaterials.Length; j++)
                {
                    highlightedMaterials[j] = originalMaterials[j];
                }

                highlightedMaterials[highlightedMaterials.Length - 1] = _highlightMaterial;
                targetRenderer.materials = highlightedMaterials;

                _activeRendererStates.Add(new RendererMaterialsState
                {
                    Renderer = targetRenderer,
                    Materials = originalMaterials
                });
            }
        }

        private void ClearHighlight()
        {
            for (int i = 0; i < _activeRendererStates.Count; i++)
            {
                RendererMaterialsState state = _activeRendererStates[i];

                if (state?.Renderer != null)
                {
                    state.Renderer.materials = state.Materials;
                }
            }

            _activeRendererStates.Clear();
        }

        private bool IsCurrentTarget(IInteractable interactable)
        {
            if (!TryGetCurrentStep(out TutorialHighlightStep step) || step.TargetInteractable == null)
            {
                return false;
            }

            return ReferenceEquals(step.TargetInteractable, interactable)
                || ContainsInteractable(step.TargetInteractable.GetComponentsInParent<MonoBehaviour>(true), interactable)
                || ContainsInteractable(step.TargetInteractable.GetComponentsInChildren<MonoBehaviour>(true), interactable);
        }

        private static bool ContainsInteractable(MonoBehaviour[] behaviours, IInteractable interactable)
        {
            for (int i = 0; i < behaviours.Length; i++)
            {
                if (behaviours[i] is IInteractable foundInteractable && ReferenceEquals(foundInteractable, interactable))
                {
                    return true;
                }
            }

            return false;
        }

        private bool TryGetCurrentStep(out TutorialHighlightStep step)
        {
            if (_currentStepIndex < 0 || _currentStepIndex >= _steps.Count)
            {
                step = null;
                return false;
            }

            step = _steps[_currentStepIndex];
            return step != null;
        }

        private bool MatchesAccident(ActiveAccident accident)
        {
            return accident != null && accident.TypeId == _accidentId;
        }
    }
}
