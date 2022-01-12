using System;
using InventoryScripts.ItemScripts;
using UnityEngine;

namespace InventoryScripts
{
    public class MoveSlot : MonoBehaviour
    {
        public bool followMouse = false;
        public Item movingItem;
        private void Update()
        {
            if (followMouse) {
                transform.position = Input.mousePosition;
            }
        }
    }
} 