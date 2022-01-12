using UnityEngine;

namespace InventoryScripts.ItemScripts
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "")]
    public class ItemData : ScriptableObject
    {
        public Sprite itemSprite;
        public string itemName;
        public string itemDescription;
        public GameObject dropPrefab;
        public GameObject equipPrefab;
        public int maxStack = 1;
    }
}
