using UnityEngine;

namespace Benjathemaker
{
    public class SimpleGemsAnim : MonoBehaviour
    {
        [Header("Rotation")]
        public bool isRotating = false;
        public bool rotateX = false;
        public bool rotateY = false;
        public bool rotateZ = false;
        public float rotationSpeed = 90f;

        [Header("Floating")]
        public bool isFloating = false;
        public bool useEasingForFloating = false;
        public float floatHeight = 1f;
        public float floatSpeed = 1f;

        [Header("Scaling")]
        public bool isScaling = false;
        public bool useEasingForScaling = false;
        public float scaleLerpSpeed = 1f;

        public Vector3 startScale;
        public Vector3 endScale;

        private Vector3 _initialLocalPosition;
        private Vector3 _initialScale;

        private float _floatTimer;
        private float _scaleTimer;

        private void Start()
        {
            _initialLocalPosition = transform.localPosition;

            _initialScale = transform.localScale;

            startScale = _initialScale;

            endScale =
                _initialScale * (endScale.magnitude / startScale.magnitude);
        }

        private void Update()
        {
            UpdateRotation();
            UpdateFloating();
            UpdateScaling();
        }

        private void UpdateRotation()
        {
            if (!isRotating)
            {
                return;
            }

            Vector3 rotationVector = new Vector3(
                rotateX ? 1f : 0f,
                rotateY ? 1f : 0f,
                rotateZ ? 1f : 0f);

            transform.Rotate(rotationVector * rotationSpeed * Time.deltaTime);
        }

        private void UpdateFloating()
        {
            if (!isFloating)
            {
                return;
            }

            _floatTimer += Time.deltaTime * floatSpeed;

            float t = Mathf.PingPong(_floatTimer, 1f);

            if (useEasingForFloating)
            {
                t = EaseInOutQuad(t);
            }

            transform.localPosition =
                _initialLocalPosition + Vector3.up * (t * floatHeight);
        }

        private void UpdateScaling()
        {
            if (!isScaling)
            {
                return;
            }

            _scaleTimer += Time.deltaTime * scaleLerpSpeed;

            float t = Mathf.PingPong(_scaleTimer, 1f);

            if (useEasingForScaling)
            {
                t = EaseInOutQuad(t);
            }

            transform.localScale =
                Vector3.Lerp(startScale, endScale, t);
        }

        private float EaseInOutQuad(float t)
        {
            return t < 0.5f
                ? 2f * t * t
                : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
        }
    }
}