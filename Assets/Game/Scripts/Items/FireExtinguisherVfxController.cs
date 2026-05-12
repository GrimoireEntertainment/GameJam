using UnityEngine;

namespace Game.Items
{
    public class FireExtinguisherVfxController : MonoBehaviour
    {
        [Header("VFX")]
        [SerializeField, Tooltip("Root object containing extinguisher spray VFX.")]
        private GameObject _sprayRoot;

        [SerializeField, Tooltip("Optional particle systems used by the spray.")]
        private ParticleSystem[] _particleSystems;

        private bool _isPlaying;

        private void Awake()
        {
            if (_particleSystems == null || _particleSystems.Length == 0)
            {
                _particleSystems = GetComponentsInChildren<ParticleSystem>(true);
            }

            StopSpray();
        }

        private void OnDisable()
        {
            StopSpray();
        }

        public void PlaySpray()
        {
            if (_isPlaying)
            {
                return;
            }

            _isPlaying = true;

            if (_sprayRoot != null)
            {
                _sprayRoot.SetActive(true);
            }

            for (int i = 0; i < _particleSystems.Length; i++)
            {
                if (_particleSystems[i] != null)
                {
                    _particleSystems[i].Play();
                }
            }
        }

        public void StopSpray()
        {
            _isPlaying = false;

            for (int i = 0; i < _particleSystems.Length; i++)
            {
                if (_particleSystems[i] != null)
                {
                    _particleSystems[i].Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
                }
            }

            if (_sprayRoot != null)
            {
                _sprayRoot.SetActive(false);
            }
        }
    }
}
