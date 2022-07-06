using System;
using General;
using InventoryScripts.ItemScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventoryScripts
{
    public class InventorySlot : MonoBehaviour {
        public static Action inventorySlotChanged = delegate {  };
        private TextMeshProUGUI _amountText;
        [SerializeField] private Transform moveParent;
        [SerializeField] private GameObject itemSlotPrefab;
        [SerializeField] private SoundEffect selectSound;
        private static GameObject _equippedItem = null;
        private Item _myItem;
        private static Transform _playerHand = null;
        private static GameObject _heldItem; //for moving items in inventory 
        public Item MyItem
        {
            get => _myItem;
            set
            {
                if (_myItem != null)
                    _myItem.ItemAmountUpdate -= UpdateAmountText;
                _myItem = value;
                if (transform.childCount != 0)
                    DestroyImmediate(transform.GetChild(0).gameObject); //using DestroyImmediate because destroy doesnt work
                if (value != null)
                {
                    _myItem.ItemAmountUpdate += UpdateAmountText;
                    GameObject itemSlot = Instantiate(itemSlotPrefab, transform);
                    _amountText = itemSlot.GetComponentInChildren<TextMeshProUGUI>();
                    itemSlot.GetComponent<Image>().sprite = _myItem.itemData.itemSprite;
                    if (value.Amount > 1)
                        _amountText.text = $"{value.Amount}";
                }
                else
                {
                    if (_amountText)
                        _amountText.text = null;
                }
            }
        }

        private void Awake() {
            if (_amountText == null)
                _amountText = GetComponentInChildren<TextMeshProUGUI>();
            if (!_playerHand)
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  _playerHand = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0);
        }

        public void EquipSlot()
        {
            if (_equippedItem)
                _equippedItem.SetActive(false);
            if (_myItem != null)
                _equippedItem = _myItem.Equip(_playerHand);
        }

        public static void ToggleEquip(bool enable)
        {
            if (_equippedItem)
                _equippedItem.SetActive(enable);
        }

        public void RemoveItem()
        {
            if (_myItem == null) return;
            _myItem.Drop();                                                                  
            MyItem = null;
        }

        public void DeleteItem()
        {
            if (_myItem == null) return;
            _myItem.Drop();
            MyItem = null;
        }

        private void UpdateAmountText(int newAmount)
        {
            if (_amountText)
                _amountText.text =  _myItem.Amount > 1 ? $"{_myItem.Amount}" : "" ;
        }

        public void SelectItem()
        {
            GameObject prevHeldItem = null;
            bool pickedUpItem = false;
            selectSound.Play(gameObject);
            
            if (transform.childCount != 0)
            {
                //try to stack items
                if (_heldItem)
                {
                    var movingItem = _heldItem.GetComponent<MoveSlot>().movingItem;
                    if (movingItem != null && _myItem != null && movingItem.itemData == _myItem.itemData) {
                        _myItem.Amount += movingItem.Amount;
                        Destroy(_heldItem);
                        return;
                    }
                }
                //pickup item
                var itemSlot = transform.GetChild(0).gameObject;
                itemSlot.transform.SetParent(moveParent, true);
                var moveSlot = itemSlot.GetComponent<MoveSlot>();
                moveSlot.description.SetActive(false);
                moveSlot.followMouse = true;
                moveSlot.movingItem = _myItem;
                prevHeldItem = _heldItem;
                _heldItem = itemSlot.gameObject;
                //update item to null now no items in this slot
                _myItem.ItemAmountUpdate -= UpdateAmountText;
                _myItem = null;
                if (_amountText)
                    _amountText = null;
                pickedUpItem = true;
            }
            
            GameObject itemToPlace = null;
            //case 1: there was a held item before pickup that will now be placed
            if (prevHeldItem)
                itemToPlace = prevHeldItem;
            //case 2: there was no pickup but a held item must be placed
            else if (_heldItem && !pickedUpItem) {
                itemToPlace = _heldItem;
                _heldItem = null;
            }
            //place item in slot
            if (itemToPlace)
            {
                itemToPlace.transform.SetParent(transform);
                itemToPlace.transform.localPosition = Vector3.zero;
                var moveSlot = itemToPlace.GetComponent<MoveSlot>();
                moveSlot.followMouse = false;
                if (moveSlot.movingItem == null) return;
                _myItem = moveSlot.movingItem;
                _myItem.ItemAmountUpdate += UpdateAmountText;
                _amountText = itemToPlace.GetComponentInChildren<TextMeshProUGUI>();
                if (_myItem.Amount > 1) {
                    _amountText.text = $"{_myItem.Amount}";
                }
            }
            inventorySlotChanged.Invoke();
        }
        
        public void TryShowDescription()
        {
            if (!_heldItem && transform.childCount > 0 && _myItem != null)
            {
                var description = transform.GetChild(0).GetComponent<MoveSlot>().description;
                description.GetComponentInChildren<TextMeshProUGUI>().text = _myItem.itemData.itemDescription;
                description.SetActive(true);
            }
        }

        public void TryHideDescription()
        {
            if (transform.childCount > 0 && _myItem != null)
            {
                transform.GetChild(0).GetComponent<MoveSlot>().description.SetActive(false);
            }
        }
    }
}