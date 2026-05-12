using System.Collections.Generic;
using Game.Audio;
using UnityEngine;

namespace Game.Core
{
    [DefaultExecutionOrder(-100)]
    public sealed class AudioService : MonoBehaviour
    {
        public static AudioService Instance { get; private set; }

        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private AudioClip _uiClickClip;
        [SerializeField] private AudioClip _winClip;
        [SerializeField] private AudioClip _loseClip;
        [SerializeField] private AudioClip _fireClip;

        [Header("Game Audio")]
        [SerializeField] private GameAudioConfig _gameAudioConfig;
        [SerializeField] private bool _playBackgroundMusicOnStart = true;

        private readonly Dictionary<GameSoundId, AudioSource> _loopSources = new();
        private readonly Dictionary<GameSoundId, int> _loopReferenceCounts = new();
        private float _musicVolume = 1f;
        private float _sfxVolume = 1f;
        private float _currentMusicSoundVolume = 1f;

        public float DefaultMusicVolume => TryGetEntry(GameSoundId.BackgroundMusic, out GameAudioConfig.SoundEntry entry) ? entry.Volume : 1f;
        public float DefaultSfxVolume => 1f;

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
            if (SettingsService.Instance != null)
            {
                SetMusicVolume(SettingsService.Instance.MusicVolume);
                SetSfxVolume(SettingsService.Instance.SfxVolume);
            }

            if (_playBackgroundMusicOnStart)
            {
                PlayMusic(GameSoundId.BackgroundMusic);
            }
        }

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (clip == null || _musicSource == null)
            {
                return;
            }

            _currentMusicSoundVolume = 1f;
            _musicSource.clip = clip;
            _musicSource.loop = loop;
            ApplyMusicVolume();
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

        public void PlaySfx(GameSoundId soundId)
        {
            if (!TryGetEntry(soundId, out GameAudioConfig.SoundEntry entry) || entry.Clip == null || _sfxSource == null)
            {
                return;
            }

            _sfxSource.PlayOneShot(entry.Clip, entry.Volume);
        }

        public void PlayMusic(GameSoundId soundId)
        {
            if (!TryGetEntry(soundId, out GameAudioConfig.SoundEntry entry) || entry.Clip == null || _musicSource == null)
            {
                return;
            }

            _currentMusicSoundVolume = soundId == GameSoundId.BackgroundMusic ? 1f : entry.Volume;
            _musicSource.clip = entry.Clip;
            _musicSource.loop = true;
            ApplyMusicVolume();
            _musicSource.Play();
        }

        public void StartLoop(GameSoundId soundId)
        {
            if (soundId == GameSoundId.None)
            {
                return;
            }

            if (_loopSources.ContainsKey(soundId))
            {
                _loopReferenceCounts[soundId] = _loopReferenceCounts.TryGetValue(soundId, out int count) ? count + 1 : 2;
                return;
            }

            if (!TryGetEntry(soundId, out GameAudioConfig.SoundEntry entry) || entry.Clip == null)
            {
                return;
            }

            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = entry.Clip;
            source.loop = true;
            source.playOnAwake = false;
            source.volume = GetScaledSfxVolume(entry.Volume);
            source.Play();
            _loopSources.Add(soundId, source);
            _loopReferenceCounts[soundId] = 1;
        }

        public void StopLoop(GameSoundId soundId)
        {
            if (!_loopSources.TryGetValue(soundId, out AudioSource source))
            {
                return;
            }

            if (_loopReferenceCounts.TryGetValue(soundId, out int count) && count > 1)
            {
                _loopReferenceCounts[soundId] = count - 1;
                return;
            }

            _loopSources.Remove(soundId);
            _loopReferenceCounts.Remove(soundId);

            if (source != null)
            {
                source.Stop();
                Destroy(source);
            }
        }

        public bool IsLoopPlaying(GameSoundId soundId)
        {
            return _loopSources.ContainsKey(soundId);
        }

        public AudioSource StartLoopInstance(GameSoundId soundId)
        {
            if (soundId == GameSoundId.None)
            {
                return null;
            }

            if (!TryGetEntry(soundId, out GameAudioConfig.SoundEntry entry) || entry.Clip == null)
            {
                return null;
            }

            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = entry.Clip;
            source.loop = true;
            source.playOnAwake = false;
            source.volume = GetScaledSfxVolume(entry.Volume);
            source.Play();
            return source;
        }

        public void StopLoopInstance(AudioSource source)
        {
            if (source == null)
            {
                return;
            }

            source.Stop();
            Destroy(source);
        }

        public void PlayUiClick()
        {
            PlaySfx(_uiClickClip);
        }
        
        public void PlayFire()
        {
            PlaySfx(_fireClip);
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
            _musicVolume = Mathf.Clamp01(volume);
            ApplyMusicVolume();
        }

        public void SetSfxVolume(float volume)
        {
            if (_sfxSource == null)
            {
                return;
            }

            _sfxVolume = Mathf.Clamp01(volume);
            _sfxSource.volume = _sfxVolume;

            foreach (KeyValuePair<GameSoundId, AudioSource> loopSource in _loopSources)
            {
                if (loopSource.Value == null)
                {
                    continue;
                }

                float soundVolume = TryGetEntry(loopSource.Key, out GameAudioConfig.SoundEntry entry) ? entry.Volume : 1f;
                loopSource.Value.volume = GetScaledSfxVolume(soundVolume);
            }
        }

        private bool TryGetEntry(GameSoundId soundId, out GameAudioConfig.SoundEntry entry)
        {
            if (_gameAudioConfig == null)
            {
                entry = null;
                return false;
            }

            return _gameAudioConfig.TryGetSound(soundId, out entry);
        }

        private float GetScaledSfxVolume(float soundVolume)
        {
            return Mathf.Clamp01(_sfxVolume * soundVolume);
        }

        private void ApplyMusicVolume()
        {
            if (_musicSource == null)
            {
                return;
            }

            _musicSource.volume = Mathf.Clamp01(_musicVolume * _currentMusicSoundVolume);
        }
    }
}
