using DG.Tweening;
using Game.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public sealed class UIButtonFeedback : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Transform _target;
        [SerializeField] private float _pressedScale = 0.95f;
        [SerializeField] private float _duration = 0.08f;

        private Vector3 _defaultScale;
        private Tween _scaleTween;

        private void Awake()
        {
            if (_button == null)
            {
                _button = GetComponent<Button>();
            }

            if (_target == null)
            {
                _target = transform;
            }

            _defaultScale = _target.localScale;
        }

        private void OnEnable()
        {
            if (_button != null)
            {
                _button.onClick.AddListener(OnClicked);
            }
        }

        private void OnDisable()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(OnClicked);
            }
        }

        private void OnDestroy()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(OnClicked);
            }

            _scaleTween?.Kill();
        }

        private void OnClicked()
        {
            AudioService.Instance?.PlayUiClick();
            PlayScaleAnimation();
        }

        private void PlayScaleAnimation()
        {
            if (_target == null)
            {
                return;
            }

            _scaleTween?.Kill();
            _target.localScale = _defaultScale;

            _scaleTween = DOTween.Sequence()
                .Append(_target.DOScale(_defaultScale * _pressedScale, _duration))
                .Append(_target.DOScale(_defaultScale, _duration));
        }
    }
}
