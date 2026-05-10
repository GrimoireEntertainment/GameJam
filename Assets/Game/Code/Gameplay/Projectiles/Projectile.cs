using Game.Core;
using Game.Gameplay.Effects;
using UnityEngine;

namespace Game.Gameplay.Projectiles
{
    public sealed class Projectile : MonoBehaviour
    {
        [SerializeField] private float _speed = 12f;
        [SerializeField] private float _lifetime = 3f;
        [SerializeField] private int _damage = 1;
        [SerializeField] private bool _destroyOnHit = true;
        [SerializeField] private GameObject _impactEffectPrefab;
        [SerializeField] private AudioClip _impactSound;

        private Vector3 _direction;
        private float _lifeTimer;
        private bool _isLaunched;

        private void OnEnable()
        {
            _lifeTimer = 0f;
        }

        private void Update()
        {
            if (!_isLaunched)
            {
                return;
            }

            transform.position += _direction * (_speed * Time.deltaTime);
            _lifeTimer += Time.deltaTime;

            if (_lifeTimer >= _lifetime)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Hit(other.gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            Hit(collision.gameObject);
        }

        public void Launch(Vector3 direction)
        {
            _direction = direction.sqrMagnitude > 0f ? direction.normalized : transform.forward;
            _isLaunched = true;
        }

        private void Hit(GameObject target)
        {
            if (target != null && target.TryGetComponent(out Health health) && !health.IsDead)
            {
                health.TakeDamage(_damage);
            }

            SpawnEffect.Spawn(_impactEffectPrefab, transform.position, transform.rotation);
            AudioService.Instance?.PlaySfx(_impactSound);

            if (_destroyOnHit)
            {
                Destroy(gameObject);
            }
        }
    }
}
