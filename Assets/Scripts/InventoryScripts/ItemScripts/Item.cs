using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InventoryScripts.ItemScripts
{
    public class Item
    {
        public ItemData itemData;
        private GameObject _equip = null;
        public Action<int> ItemAmountUpdate = delegate { };
        private int _amount = 1;

        public int Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                ItemAmountUpdate.Invoke(value);
            }
        }

        public Item(ItemData itemData)
        {
            this.itemData = itemData;
        }

        public Item(ItemData itemData, int amount)
        {
            this.itemData = itemData;
            _amount = amount;
        }
        
        public virtual GameObject Equip(Transform playerHand)
        {
            if (_equip)
                _equip.SetActive(true);
            else if (itemData.equipPrefab)
            {
                _equip = Object.Instantiate(itemData.equipPrefab, playerHand);
                _equip.transform.localPosition = Vector3.zero;
                _equip.transform.localRotation = Quaternion.identity;
                var consumable = _equip.GetComponent<Consumable>();
                if (consumable)
                {
                     consumable.itemConsumed += delegate
                     {
                         Inventory.Instance.DestroyItem(this);
                         if (_amount == 0)
                             Object.Destroy(_equip);
                     };
                }
            }
            return _equip;
        }

        public virtual void Drop() 
        {
            //TODO throw dropItem away from player
            if (_equip)
                Object.Destroy(_equip);
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var drop  = Object.Instantiate(itemData.dropPrefab, new Vector3(mousePos.x, mousePos.y, 0),
                Quaternion.identity);
            drop.GetComponent<ItemPickup>().itemToDrop = this;
        }

        public virtual void DeleteItem()
        {
            Inventory.Instance.DestroyItem(this, _amount);
        }
    }
}