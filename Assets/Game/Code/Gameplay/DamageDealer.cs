using UnityEngine;

namespace Game.Gameplay
{
    public sealed class DamageDealer : MonoBehaviour
    {
        [SerializeField] private int _damage = 1;
        [SerializeField] private bool _destroyAfterDamage = false;

        private void OnTriggerEnter(Collider other)
        {
            TryDealDamage(other.gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            TryDealDamage(collision.gameObject);
        }

        private void TryDealDamage(GameObject target)
        {
            if (target == null)
            {
                return;
            }

            Health health = target.GetComponent<Health>();

            if (health == null || health.IsDead)
            {
                return;
            }

            health.TakeDamage(_damage);

            if (_destroyAfterDamage)
            {
                Destroy(gameObject);
            }
        }
    }
}
