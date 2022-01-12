using System;
using UnityEngine;

namespace InventoryScripts
{
    public class Trash : MonoBehaviour
    {
        public static Action TrashItem = delegate { };
        private void Update()
        {
            if(Vector2.Distance(Input.mousePosition, transform.position) < 20)
                TrashItem();
        }
    }
}
