using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Audio
{
    [CreateAssetMenu(fileName = "GameAudioConfig", menuName = "Game/Audio/Game Audio Config")]
    public class GameAudioConfig : ScriptableObject
    {
        [Serializable]
        public class SoundEntry
        {
            [Tooltip("Gameplay sound id.")]
            public GameSoundId SoundId;

            [Tooltip("Audio clip played for this sound.")]
            public AudioClip Clip;

            [Range(0f, 1f), Tooltip("Per-sound volume multiplier.")]
            public float Volume = 1f;
        }

        [SerializeField, Tooltip("Sound clips used by the game.")]
        private List<SoundEntry> _sounds = new();

        public bool TryGetSound(GameSoundId soundId, out SoundEntry sound)
        {
            for (int i = _sounds.Count - 1; i >= 0; i--)
            {
                SoundEntry entry = _sounds[i];

                if (entry != null && entry.SoundId == soundId)
                {
                    sound = entry;
                    return true;
                }
            }

            sound = null;
            return false;
        }
    }
}
