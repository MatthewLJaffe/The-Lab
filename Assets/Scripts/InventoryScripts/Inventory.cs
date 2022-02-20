using System;
using System.Collections.Generic;
using General;
using InventoryScripts.ItemScripts;
using PlayerScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventoryScripts
{
    [RequireComponent(typeof(Toggler))]
    public class Inventory : MonoBehaviour
    {
        public static Action OnInventoryUpdated = delegate { };
        public List<InventorySlot> slotList;
        public List<Item> itemList;
        private int _currentlyEquipped;
        private GameObject[] _hotBar;
        [SerializeField] private GameObject craftingParent;
        [SerializeField] private Color equipColor;
        [SerializeField] private Color defaultColor;
        public static Inventory Instance;
        private Toggler _toggler;

        private void Awake()
        {
            ItemPickup.pickup += AddItem;
            PlayerInputManager.OnInputDown += ToggleInventory;
            PlayerInputManager.OnInputDown += NumberInput;
            InventorySlot.InventorySlotChanged += UpdateInventorySlots;
            
            _toggler = GetComponent<Toggler>();
            _hotBar = new GameObject[transform.GetChild(1).childCount];
            for(int i = 0; i < _hotBar.Length; i++) {
                _hotBar[i] = transform.GetChild(1).GetChild(i).gameObject;
            }
            slotList = new List<InventorySlot>();
            itemList = new List<Item>();
            UpdateInventorySlots();
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }

        private void OnDestroy()
        {
            PlayerInputManager.OnInputDown -= ToggleInventory;
            PlayerInputManager.OnInputDown -= NumberInput;
            InventorySlot.InventorySlotChanged -= UpdateInventorySlots;
            ItemPickup.pickup -= AddItem;

            _hotBar.Initialize();
            slotList.Clear();
        }

        public void ToggleCrafting()
        {
            craftingParent.SetActive(!craftingParent.activeSelf);
        }

        public void SetCraftingEnabled(bool enable)
        {
            craftingParent.SetActive(enable);
        }

        public InventorySlot GetSlotFromItem(Item item)
        {
            return slotList.Find(slot => slot.MyItem == item);
        }

        public void AddItem(Item item)
        {
            AddItem(item, true);
        }

        public void AddItem(Item item, bool stackItem)
        {
            //increase stack count on item if it already exists in inventory
            var existingSlot = slotList.Find(s => s.MyItem != null && s.MyItem.itemData == item.itemData 
                                                   && s.MyItem.Amount < s.MyItem.itemData.maxStack);
            if (existingSlot && stackItem)
            {
                var slotAddition = Mathf.Clamp(item.Amount, 0, item.itemData.maxStack - existingSlot.MyItem.Amount);
                item.Amount -= slotAddition;
                existingSlot.MyItem.Amount += slotAddition;
                if (item.Amount == 0)
                {
                    item = null;
                    UpdateInventorySlots();
                    return;
                }
            }
            
            itemList.Add(item);
            for (int i = 0; i < slotList.Count; i++) 
            {
                if(slotList[i].MyItem == null) {
                    slotList[i].MyItem = item;
                    if (_currentlyEquipped == i)
                        slotList[i].EquipSlot();
                    UpdateInventorySlots();
                    return;
                }
            }
        }

        public void SwapItems(InventorySlot slot1, InventorySlot slot2)
        {
            if (slot1.MyItem == null && slot2.MyItem == null)
                return;
            
        }

        public void DestroyItem(Item item, int amount = 1)
        {
            var removeSlot = slotList.Find(iSlot => iSlot.MyItem == item);
            if (removeSlot.MyItem.Amount > amount)
                removeSlot.MyItem.Amount -= amount;
            else
            {
                removeSlot.MyItem.Amount = 0;
                itemList.Remove(item);
                removeSlot.MyItem = null;
            }
            UpdateHotBar();
            OnInventoryUpdated.Invoke();
        }
        
        public void UpdateInventorySlots()
        {
            slotList.Clear();
            itemList.Clear();
            var slots = transform.GetChild(0).GetComponentsInChildren<InventorySlot>();
            foreach (var slot in slots)
            {
                slotList.Add(slot);
                if (slot.MyItem != null)
                    itemList.Add(slot.MyItem);
            }
            slotList[_currentlyEquipped].EquipSlot();
            UpdateHotBar();
            OnInventoryUpdated.Invoke();
        }

        public void ToggleInventory()
        {
            _toggler.ToggleItems();
        }
        
        private void ToggleInventory(PlayerInputManager.PlayerInputName iName)
        {
            if (iName == PlayerInputManager.PlayerInputName.Inventory)
                _toggler.ToggleItems();
        }

        private void NumberInput(PlayerInputManager.PlayerInputName iName)
        {
            if (!iName.ToString().StartsWith("Alpha_")) return;
            int slotIndex = int.Parse(iName.ToString().Split('_')[1]) - 1;
                SetColor(_hotBar[_currentlyEquipped], defaultColor);
                slotList[slotIndex].EquipSlot();
                _currentlyEquipped = slotIndex;
                SetColor(_hotBar[_currentlyEquipped], equipColor);
        }

        private void UpdateHotBar()
        {
            for (int i = 0; i < _hotBar.Length; i++) {
                if (slotList[i].MyItem != null)
                {
                    _hotBar[i].GetComponent<Image>().sprite = slotList[i].MyItem.itemData.itemSprite;
                    var tmp = _hotBar[i].GetComponentInChildren<TextMeshProUGUI>();
                    tmp.text = slotList[i].MyItem.Amount > 1 ? $"{slotList[i].MyItem.Amount}" : "";
                }
                else {
                    _hotBar[i].transform.GetComponent<Image>().sprite = null;
                    var tmp = _hotBar[i].GetComponentInChildren<TextMeshProUGUI>().text = "";
                }
            }
        }

        private void SetColor(GameObject slot, Color color) =>
            slot.GetComponent<Image>().color = color;

        private void OnDisable()
        {
            craftingParent.SetActive(false);
        }
    }
    
}