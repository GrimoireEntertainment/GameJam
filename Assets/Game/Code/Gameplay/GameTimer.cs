using UnityEngine;

namespace Game.Gameplay
{
    public enum TimerMode
    {
        CountUp,
        CountDown
    }

    public sealed class GameTimer : MonoBehaviour
    {
        [SerializeField] private TimerMode _mode = TimerMode.CountUp;
        [SerializeField] private float _duration = 180f;

        public float ElapsedTime { get; private set; }
        public float RemainingTime => Mathf.Max(0f, _duration - ElapsedTime);
        public bool IsRunning { get; private set; }
        public TimerMode Mode => _mode;

        private GameSessionController _gameSessionController;

        private void Start()
        {
            _gameSessionController = GameSessionController.Instance;

            if (_gameSessionController != null)
            {
                _gameSessionController.StateChanged += OnStateChanged;
            }

            StartTimer();
        }

        private void Update()
        {
            if (!IsRunning)
            {
                return;
            }

            ElapsedTime += Time.deltaTime;

            if (_mode == TimerMode.CountDown && RemainingTime <= 0f)
            {
                StopTimer();

                if (GameSessionController.Instance != null)
                {
                    GameSessionController.Instance.Lose();
                }
            }
        }

        private void OnDestroy()
        {
            if (_gameSessionController != null)
            {
                _gameSessionController.StateChanged -= OnStateChanged;
            }
        }

        public void StartTimer()
        {
            if (_mode == TimerMode.CountDown && RemainingTime <= 0f)
            {
                ResetTimer();
            }

            IsRunning = true;
        }

        public void StopTimer()
        {
            IsRunning = false;
        }

        public void ResetTimer()
        {
            ElapsedTime = 0f;
        }

        private void OnStateChanged(GameSessionState state)
        {
            if (state == GameSessionState.Won || state == GameSessionState.Lost)
            {
                StopTimer();
            }
        }
    }
}
