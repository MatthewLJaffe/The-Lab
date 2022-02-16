using InventoryScripts.ItemScripts;
using UnityEngine;

namespace InventoryScripts
{
    [CreateAssetMenu(fileName = "CraftingRecipe", menuName = "")]
    public class CraftingRecipe : ScriptableObject
    {
        public CraftType craftWith;
        public ItemAmountPair[] ingredients;
        public ItemAmountPair result;
        
        public enum CraftType
        {
            Hands,
            WorkBench
        }
        [System.Serializable]
        public struct ItemAmountPair
        {
            public ItemData itemData;
            [MinAttribute(1)]
            public int amount;
        }
    }
}