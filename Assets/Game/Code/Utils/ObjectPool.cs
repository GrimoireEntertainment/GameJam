using System.Collections.Generic;
using UnityEngine;

namespace Game.Utils
{
    public sealed class ObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _initialSize = 10;
        [SerializeField] private Transform _container;

        private readonly Queue<GameObject> _pool = new();

        private void Awake()
        {
            if (_container == null)
            {
                _container = transform;
            }
        }

        public GameObject Get()
        {
            if (_prefab == null)
            {
                Debug.LogWarning("ObjectPool prefab is not assigned.", this);
                return null;
            }

            GameObject obj = _pool.Count > 0 ? _pool.Dequeue() : CreateObject();
            obj.SetActive(true);
            return obj;
        }

        public void Release(GameObject obj)
        {
            if (obj == null)
            {
                return;
            }

            obj.SetActive(false);
            obj.transform.SetParent(_container);
            _pool.Enqueue(obj);
        }

        public void Prewarm()
        {
            if (_prefab == null)
            {
                Debug.LogWarning("ObjectPool prefab is not assigned.", this);
                return;
            }

            for (int i = 0; i < _initialSize; i++)
            {
                Release(CreateObject());
            }
        }

        private GameObject CreateObject()
        {
            GameObject obj = Instantiate(_prefab, _container);
            obj.SetActive(false);
            return obj;
        }
    }
}
