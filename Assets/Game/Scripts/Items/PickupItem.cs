using UnityEngine;

namespace Game.Items
{
    public class PickupItem : MonoBehaviour
    {
        [Header("Item")]
        [SerializeField] private ItemType _itemType = ItemType.None;

        public ItemType ItemType => _itemType;

        public void Setup(ItemType itemType)
        {
            _itemType = itemType;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.collider.CompareTag("Player")) return;

            Destroy(gameObject);
        }
    }
}
