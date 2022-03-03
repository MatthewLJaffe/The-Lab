using System;
using InventoryScripts.ItemScripts;
using UnityEngine;

namespace InventoryScripts
{
    public class MoveSlot : MonoBehaviour
    {
        public bool followMouse = false;
        public Item movingItem;

        private void Awake()
        {
            Trash.TrashItem += TrashSelectedItem;
        }

        private void OnDestroy()
        {
            Trash.TrashItem -= TrashSelectedItem;
        }
 
        private void Update()
        {
            if (followMouse) {
                transform.position = Input.mousePosition;
            }
        }

        private void TrashSelectedItem()
        {
            if (followMouse) {
                movingItem.DeleteItem();
                movingItem = null;
                Destroy(gameObject);
            }
        }
    }
} 