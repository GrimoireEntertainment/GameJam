using System.Collections;
using Game.Audio;
using Game.Core;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class TutorialDialogController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Tooltip("Root object of the dialog window.")]
        private GameObject _root;

        [SerializeField, Tooltip("Single text field used by this dialog.")]
        private TMP_Text _text;

        [SerializeField, Tooltip("RectTransform moved when a custom position is requested.")]
        private RectTransform _dialogRect;

        [Header("Typewriter")]
        [SerializeField, Tooltip("Show text with a fast typewriter effect.")]
        private bool _useTypewriter = true;

        [SerializeField, Tooltip("Characters revealed per second.")]
        private float _charactersPerSecond = 45f;

        private Coroutine _hideRoutine;
        private Coroutine _typeRoutine;

        private void Awake()
        {
            if (_dialogRect == null)
            {
                _dialogRect = _root != null ? _root.GetComponent<RectTransform>() : GetComponent<RectTransform>();
            }

            Hide();
        }

        public void Show(string message)
        {
            Show(message, null, 0f);
        }

        public void Show(string message, RectTransform positionTarget)
        {
            Show(message, positionTarget, 0f);
        }

        public void Show(string message, RectTransform positionTarget, float duration)
        {
            if (_hideRoutine != null)
            {
                StopCoroutine(_hideRoutine);
                _hideRoutine = null;
            }

            if (_typeRoutine != null)
            {
                StopCoroutine(_typeRoutine);
                _typeRoutine = null;
            }

            if (_root != null)
            {
                _root.SetActive(true);
            }

            AudioService.Instance?.PlaySfx(GameSoundId.Notify);

            if (_text != null)
            {
                if (_useTypewriter && _charactersPerSecond > 0f)
                {
                    _typeRoutine = StartCoroutine(TypeRoutine(message));
                }
                else
                {
                    _text.text = message;
                }
            }

            if (_dialogRect != null && positionTarget != null)
            {
                _dialogRect.anchorMin = positionTarget.anchorMin;
                _dialogRect.anchorMax = positionTarget.anchorMax;
                _dialogRect.pivot = positionTarget.pivot;
                _dialogRect.anchoredPosition = positionTarget.anchoredPosition;
            }

            if (duration > 0f)
            {
                _hideRoutine = StartCoroutine(HideAfterDelay(duration));
            }
        }

        public void ShowAtAnchoredPosition(string message, Vector2 anchoredPosition)
        {
            ShowAtAnchoredPosition(message, anchoredPosition, 0f);
        }

        public void ShowAtAnchoredPosition(string message, Vector2 anchoredPosition, float duration)
        {
            if (_dialogRect != null)
            {
                _dialogRect.anchoredPosition = anchoredPosition;
            }

            Show(message, null, duration);
        }

        public void Hide()
        {
            if (_hideRoutine != null)
            {
                StopCoroutine(_hideRoutine);
                _hideRoutine = null;
            }

            if (_typeRoutine != null)
            {
                StopCoroutine(_typeRoutine);
                _typeRoutine = null;
            }

            if (_root != null)
            {
                _root.SetActive(false);
            }
        }

        private System.Collections.IEnumerator HideAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            _hideRoutine = null;
            Hide();
        }

        private IEnumerator TypeRoutine(string message)
        {
            _text.text = string.Empty;

            float secondsPerCharacter = 1f / _charactersPerSecond;
            WaitForSeconds delay = new(secondsPerCharacter);

            for (int i = 0; i < message.Length; i++)
            {
                _text.text = message.Substring(0, i + 1);
                yield return delay;
            }

            _typeRoutine = null;
        }
    }
}
