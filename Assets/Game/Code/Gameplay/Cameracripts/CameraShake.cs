using System.Collections;
using UnityEngine;

namespace Game.Code.Gameplay.Cameracripts
{
    public sealed class CameraShake : MonoBehaviour
    {
        [SerializeField] private float _smallDuration = 0.12f;
        [SerializeField] private float _smallStrength = 0.08f;
        [SerializeField] private float _mediumDuration = 0.2f;
        [SerializeField] private float _mediumStrength = 0.18f;

        private Coroutine _shakeRoutine;

        public void Shake(float duration, float strength)
        {
            if (_shakeRoutine != null)
            {
                StopCoroutine(_shakeRoutine);
            }

            _shakeRoutine = StartCoroutine(ShakeRoutine(duration, strength));
        }

        public void ShakeSmall()
        {
            Shake(_smallDuration, _smallStrength);
        }

        public void ShakeMedium()
        {
            Shake(_mediumDuration, _mediumStrength);
        }

        private IEnumerator ShakeRoutine(float duration, float strength)
        {
            Vector3 startLocalPosition = transform.localPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                Vector3 offset = Random.insideUnitSphere * strength;
                transform.localPosition = startLocalPosition + offset;

                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = startLocalPosition;
            _shakeRoutine = null;
        }
    }
}
