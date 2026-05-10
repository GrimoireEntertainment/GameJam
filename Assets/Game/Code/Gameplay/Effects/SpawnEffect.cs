using UnityEngine;

namespace Game.Gameplay.Effects
{
    public static class SpawnEffect
    {
        public static void Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (prefab == null)
            {
                return;
            }

            Object.Instantiate(prefab, position, rotation);
        }
    }
}
