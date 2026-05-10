using UnityEngine;

namespace Game.Core
{
    public sealed class SettingsService : MonoBehaviour
    {
        private const string MusicVolumeKey = "Settings.MusicVolume";
        private const string SfxVolumeKey = "Settings.SfxVolume";

        public static SettingsService Instance { get; private set; }

        public float MusicVolume { get; private set; } = 1f;
        public float SfxVolume { get; private set; } = 1f;
        public bool Fullscreen => true;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }

        private void Start()
        {
            ApplySettings();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void Load()
        {
            MusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
            SfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);

            ApplySettings();
        }

        public void Save()
        {
            PlayerPrefs.SetFloat(MusicVolumeKey, MusicVolume);
            PlayerPrefs.SetFloat(SfxVolumeKey, SfxVolume);
            PlayerPrefs.Save();
        }

        public void SetMusicVolume(float value)
        {
            MusicVolume = Mathf.Clamp01(value);
            AudioService.Instance?.SetMusicVolume(MusicVolume);
        }

        public void SetSfxVolume(float value)
        {
            SfxVolume = Mathf.Clamp01(value);
            AudioService.Instance?.SetSfxVolume(SfxVolume);
        }

        private void ApplySettings()
        {
            AudioService.Instance?.SetMusicVolume(MusicVolume);
            AudioService.Instance?.SetSfxVolume(SfxVolume);
            Screen.fullScreen = true;
        }
    }
}
