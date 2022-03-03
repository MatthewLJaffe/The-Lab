using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventoryScripts
{
    public class CraftableSlot : MonoBehaviour
    {
        public TextMeshProUGUI amountText;
        public Image img;
        public CraftingRecipe myRecipe;

        public Sprite SlotSprite
        {
            get => img.sprite;
            set
            {
                img.sprite = value;
                img.color = value == null ? new Color(1f,1f,1f,0f) : Color.white;
            }
        }

    }
    
    
}