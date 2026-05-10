using System.Collections.Generic;
using UnityEngine;

namespace Game.Utils
{
    public sealed class ObjectPoolExample : MonoBehaviour
    {
        [SerializeField] private ObjectPool _pool;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Vector3 _launchDirection = Vector3.up;
        [SerializeField] private float _launchForce = 3f;
        [SerializeField] private bool _autoSpawn = true;
        [SerializeField] private float _spawnInterval = 0.5f;
        [SerializeField] private int _maxActiveObjects = 10;

        private readonly List<GameObject> _spawnedObjects = new();
        private float _spawnTimer;

        private void Start()
        {
            Prewarm();
        }

        private void Update()
        {
            if (!_autoSpawn)
            {
                return;
            }

            _spawnTimer += Time.deltaTime;

            if (_spawnTimer < _spawnInterval)
            {
                return;
            }

            _spawnTimer = 0f;
            Spawn();
        }

        public void Prewarm()
        {
            if (_pool != null)
            {
                _pool.Prewarm();
            }
        }

        public void Spawn()
        {
            if (_pool == null)
            {
                return;
            }

            GameObject obj = _pool.Get();

            if (obj == null)
            {
                return;
            }

            if (_maxActiveObjects > 0 && _spawnedObjects.Count >= _maxActiveObjects)
            {
                ReleaseOldest();
            }

            if (_spawnPoint != null)
            {
                obj.transform.SetPositionAndRotation(_spawnPoint.position, _spawnPoint.rotation);
            }

            Launch(obj);
            _spawnedObjects.Add(obj);
        }

        public void ReleaseLast()
        {
            if (_pool == null || _spawnedObjects.Count == 0)
            {
                return;
            }

            int lastIndex = _spawnedObjects.Count - 1;
            GameObject obj = _spawnedObjects[lastIndex];
            _spawnedObjects.RemoveAt(lastIndex);
            _pool.Release(obj);
        }

        public void ReleaseOldest()
        {
            if (_pool == null || _spawnedObjects.Count == 0)
            {
                return;
            }

            GameObject obj = _spawnedObjects[0];
            _spawnedObjects.RemoveAt(0);
            _pool.Release(obj);
        }

        public void ReleaseAll()
        {
            if (_pool == null)
            {
                return;
            }

            for (int i = _spawnedObjects.Count - 1; i >= 0; i--)
            {
                _pool.Release(_spawnedObjects[i]);
            }

            _spawnedObjects.Clear();
        }

        private void Launch(GameObject obj)
        {
            Rigidbody rigidbody = obj.GetComponent<Rigidbody>();

            if (rigidbody == null)
            {
                return;
            }

            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.AddForce(_launchDirection.normalized * _launchForce, ForceMode.Impulse);
        }
    }
}
