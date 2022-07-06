using System;
using PlayerScripts;
using UnityEngine;

namespace InventoryScripts
{
    public class CraftingStation : MonoBehaviour, IInteractable
    {
        public static Action<CraftingRecipe.CraftType> craftTypeChange;
        [SerializeField] private CraftingRecipe.CraftType craftType;
        [SerializeField] private Color highlightColor;
        [SerializeField] private SpriteRenderer sr;
        public bool CanInteract
        {
            set
            {
                if (value) {
                    craftTypeChange.Invoke(craftType);
                    sr.color = highlightColor;
                }
                else
                {
                    sr.color = Color.white;
                    craftTypeChange.Invoke(CraftingRecipe.CraftType.Hands);
                }
            }
        }

        public void Interact()
        {
            Inventory.Instance.SetCraftingEnabled(true);
            Inventory.Instance.ToggleInventory();
        }
    }
}