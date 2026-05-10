using System;
using Game.Items;

namespace Game.Crafting
{
    [Serializable]
    public class CraftRecipe
    {
        public ItemType First;
        public ItemType Second;
        public ItemType Result;

        public bool Matches(ItemType a, ItemType b)
        {
            return First == a && Second == b || First == b && Second == a;
        }
    }
}
