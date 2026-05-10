using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Items
{
    [CreateAssetMenu(fileName = "ItemPrefabDatabase", menuName = "Game/Items/Item Prefab Database")]
    public class ItemPrefabDatabase : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            [Tooltip("Item type this prefab represents.")]
            public ItemType ItemType;

            [Tooltip("PickupItem prefab spawned for this item type.")]
            public PickupItem Prefab;
        }

        [Header("Prefabs")]
        [SerializeField] private List<Entry> _entries = new();

        public PickupItem GetPrefab(ItemType type)
        {
            if (type == ItemType.None)
            {
                return null;
            }

            for (int i = 0; i < _entries.Count; i++)
            {
                Entry entry = _entries[i];

                if (entry != null && entry.ItemType == type)
                {
                    return entry.Prefab;
                }
            }

            return null;
        }
    }
}
