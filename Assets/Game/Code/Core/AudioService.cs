using UnityEngine;

namespace Game.Core
{
    public sealed class AudioService : MonoBehaviour
    {
        public static AudioService Instance { get; private set; }

        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private AudioClip _uiClickClip;
        [SerializeField] private AudioClip _winClip;
        [SerializeField] private AudioClip _loseClip;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (SettingsService.Instance == null)
            {
                return;
            }

            SetMusicVolume(SettingsService.Instance.MusicVolume);
            SetSfxVolume(SettingsService.Instance.SfxVolume);
        }

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (clip == null || _musicSource == null)
            {
                return;
            }

            _musicSource.clip = clip;
            _musicSource.loop = loop;
            _musicSource.Play();
        }

        public void StopMusic()
        {
            if (_musicSource == null)
            {
                return;
            }

            _musicSource.Stop();
        }

        public void PlaySfx(AudioClip clip)
        {
            if (clip == null || _sfxSource == null)
            {
                return;
            }

            _sfxSource.PlayOneShot(clip);
        }

        public void PlayUiClick()
        {
            PlaySfx(_uiClickClip);
        }

        public void PlayWin()
        {
            PlaySfx(_winClip);
        }

        public void PlayLose()
        {
            PlaySfx(_loseClip);
        }

        public void SetMusicVolume(float volume)
        {
            if (_musicSource == null)
            {
                return;
            }

            _musicSource.volume = Mathf.Clamp01(volume);
        }

        public void SetSfxVolume(float volume)
        {
            if (_sfxSource == null)
            {
                return;
            }

            _sfxSource.volume = Mathf.Clamp01(volume);
        }
    }
}
