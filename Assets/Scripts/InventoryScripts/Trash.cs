using System;
using UnityEngine;

namespace InventoryScripts
{
    public class Trash : MonoBehaviour
    {
        public static Action TrashItem = delegate { };

        public void InvokeTrash()
        {
            TrashItem.Invoke();
        }
    }
}
