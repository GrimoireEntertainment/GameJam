using Game.Utils;
using UnityEngine;

namespace Game.Gameplay.Projectiles
{
    public sealed class ProjectileSpawner : MonoBehaviour
    {
        [SerializeField] private Projectile _projectilePrefab;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private float _fireRate = 0.2f;
        [SerializeField] private bool _useObjectPool = false;
        [SerializeField] private ObjectPool _objectPool;

        private float _nextFireTime;

        public void Fire()
        {
            Fire(transform.forward);
        }

        public void Fire(Vector3 direction)
        {
            if (Time.time < _nextFireTime)
            {
                return;
            }

            Transform firePoint = _firePoint != null ? _firePoint : transform;
            Projectile projectile = CreateProjectile(firePoint);

            if (projectile == null)
            {
                return;
            }

            _nextFireTime = Time.time + _fireRate;
            projectile.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);
            projectile.Launch(direction);
        }

        private Projectile CreateProjectile(Transform firePoint)
        {
            if (_useObjectPool && _objectPool != null)
            {
                GameObject pooledObject = _objectPool.Get();
                return pooledObject != null ? pooledObject.GetComponent<Projectile>() : null;
            }

            if (_projectilePrefab == null)
            {
                return null;
            }

            return Instantiate(_projectilePrefab, firePoint.position, firePoint.rotation);
        }
    }
}
