using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public sealed class LoadingScreen : MonoBehaviour
    {
        public static LoadingScreen Instance { get; private set; }

        [SerializeField] private GameObject _root;
        [SerializeField] private Slider _progressSlider;
        [SerializeField] private TMP_Text _progressText;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            Hide();
        }

        public void Show()
        {
            SetProgress(0f);

            if (_root != null)
            {
                _root.SetActive(true);
            }
        }

        public void Hide()
        {
            if (_root != null)
            {
                _root.SetActive(false);
            }
        }

        public void SetProgress(float value)
        {
            float progress = Mathf.Clamp01(value);

            if (_progressSlider != null)
            {
                _progressSlider.value = progress;
            }

            if (_progressText != null)
            {
                _progressText.text = $"{Mathf.RoundToInt(progress * 100f)}%";
            }
        }
    }
}
