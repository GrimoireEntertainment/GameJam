using System;
using UnityEngine;

namespace Game.Gameplay
{
    public sealed class ScoreController : MonoBehaviour
    {
        public static ScoreController Instance { get; private set; }

        public event Action<int> ScoreChanged;

        public int Score { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void AddScore(int amount)
        {
            SetScore(Score + amount);
        }

        public void SetScore(int value)
        {
            int newScore = Mathf.Max(0, value);

            if (Score == newScore)
            {
                return;
            }

            Score = newScore;
            ScoreChanged?.Invoke(Score);
        }

        public void ResetScore()
        {
            SetScore(0);
        }
    }
}
