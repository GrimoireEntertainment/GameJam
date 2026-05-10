using Game.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public sealed class SettingsPopup : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _sfxSlider;

        private void OnEnable()
        {
            if (_musicSlider != null)
            {
                _musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            }

            if (_sfxSlider != null)
            {
                _sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
            }

            RefreshUi();
        }

        private void OnDisable()
        {
            if (_musicSlider != null)
            {
                _musicSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
            }

            if (_sfxSlider != null)
            {
                _sfxSlider.onValueChanged.RemoveListener(OnSfxVolumeChanged);
            }
        }

        public void Show()
        {
            if (_root != null)
            {
                _root.SetActive(true);
            }

            RefreshUi();
        }

        public void Hide()
        {
            if (_root != null)
            {
                _root.SetActive(false);
            }
        }

        public void OnMusicVolumeChanged(float value)
        {
            if (SettingsService.Instance == null)
            {
                return;
            }

            SettingsService.Instance.SetMusicVolume(value);
            SettingsService.Instance.Save();
        }

        public void OnSfxVolumeChanged(float value)
        {
            if (SettingsService.Instance == null)
            {
                return;
            }

            SettingsService.Instance.SetSfxVolume(value);
            SettingsService.Instance.Save();
        }

        private void RefreshUi()
        {
            if (SettingsService.Instance == null)
            {
                return;
            }

            if (_musicSlider != null)
            {
                _musicSlider.SetValueWithoutNotify(SettingsService.Instance.MusicVolume);
            }

            if (_sfxSlider != null)
            {
                _sfxSlider.SetValueWithoutNotify(SettingsService.Instance.SfxVolume);
            }
        }
    }
}
