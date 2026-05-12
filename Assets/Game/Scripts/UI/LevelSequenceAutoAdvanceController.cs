using Game.Level;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class LevelSequenceAutoAdvanceController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField, Tooltip("Optional fake loading fill image.")]
        private Image _progressFillImage;

        [SerializeField, Tooltip("Optional countdown text.")]
        private TMP_Text _countdownText;

        [Header("Timing")]
        [SerializeField, Tooltip("Seconds before loading the next scene in the level sequence.")]
        private float _duration = 5f;

        [SerializeField, Tooltip("Countdown text format. {0} is replaced with remaining seconds.")]
        private string _countdownFormat = "{0}";

        [SerializeField, Tooltip("Scale used when a new countdown number appears.")]
        private float _countdownStartScale = 0.2f;

        [SerializeField, Tooltip("Seconds used for the countdown number to grow to full size.")]
        private float _countdownScaleInDuration = 0.12f;

        [SerializeField, Tooltip("Seconds used for the countdown number to fade out.")]
        private float _countdownFadeOutDuration = 0.75f;

        private int _lastCountdownValue = -1;
        private float _timer;
        private float _countdownNumberTimer;
        private bool _hasLoadedNextScene;

        private void OnEnable()
        {
            _timer = 0f;
            _countdownNumberTimer = 0f;
            _lastCountdownValue = -1;
            _hasLoadedNextScene = false;
            SetProgress(0f);
            UpdateCountdown(force: true);
        }

        private void Update()
        {
            if (_hasLoadedNextScene)
            {
                return;
            }

            _timer += Time.unscaledDeltaTime;
            _countdownNumberTimer += Time.unscaledDeltaTime;
            float progress = _duration > 0f ? Mathf.Clamp01(_timer / _duration) : 1f;

            SetProgress(progress);
            UpdateCountdown(force: false);
            UpdateCountdownAnimation();

            if (progress >= 1f)
            {
                _hasLoadedNextScene = true;
                LevelSequenceController.Instance?.LoadNextScene();
            }
        }

        private void SetProgress(float progress)
        {
            if (_progressFillImage != null)
            {
                _progressFillImage.fillAmount = progress;
            }
        }

        private void UpdateCountdown(bool force)
        {
            if (_countdownText == null)
            {
                return;
            }

            int secondsLeft = Mathf.Max(0, Mathf.CeilToInt(_duration - _timer));

            if (!force && secondsLeft == _lastCountdownValue)
            {
                return;
            }

            _lastCountdownValue = secondsLeft;
            _countdownNumberTimer = 0f;
            _countdownText.text = string.Format(_countdownFormat, secondsLeft);
            _countdownText.transform.localScale = Vector3.one * Mathf.Max(0f, _countdownStartScale);
            SetCountdownAlpha(1f);
        }

        private void UpdateCountdownAnimation()
        {
            if (_countdownText == null)
            {
                return;
            }

            float scaleProgress = _countdownScaleInDuration > 0f
                ? Mathf.Clamp01(_countdownNumberTimer / _countdownScaleInDuration)
                : 1f;

            float scale = Mathf.Lerp(Mathf.Max(0f, _countdownStartScale), 1f, scaleProgress);
            _countdownText.transform.localScale = Vector3.one * scale;

            float fadeProgress = _countdownFadeOutDuration > 0f
                ? Mathf.Clamp01(_countdownNumberTimer / _countdownFadeOutDuration)
                : 1f;

            SetCountdownAlpha(1f - fadeProgress);
        }

        private void SetCountdownAlpha(float alpha)
        {
            if (_countdownText == null)
            {
                return;
            }

            Color color = _countdownText.color;
            color.a = Mathf.Clamp01(alpha);
            _countdownText.color = color;
        }
    }
}
