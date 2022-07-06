using InventoryScripts.ItemScripts;
using UnityEngine;

namespace InventoryScripts
{
    [CreateAssetMenu(fileName = "CraftingRecipe", menuName = "")]
    public class CraftingRecipe : ScriptableObject
    {
        public bool useAnyIngredient;
        public CraftType craftWith;
        public ItemAmountPair[] ingredients;
        public ItemAmountPair result;
        
        public enum CraftType
        {
            Hands,
            WorkBench,
            Duplicator
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