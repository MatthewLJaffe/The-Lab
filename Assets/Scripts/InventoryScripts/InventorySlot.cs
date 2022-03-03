using System;
using InventoryScripts.ItemScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventoryScripts
{
    public class InventorySlot : MonoBehaviour {
        public static Action InventorySlotChanged = delegate {  };
        private TextMeshProUGUI amountText;
        [SerializeField] private Transform moveParent;
        [SerializeField] private GameObject itemSlotPrefab;
        private static GameObject equippedItem = null;
        private Item myItem;
        private static Transform playerHand = null;
        private static GameObject heldItem; //for moving items in inventory 
        public Item MyItem
        {
            get => myItem;
            set
            {
                if (myItem != null)
                    myItem.ItemAmountUpdate -= UpdateAmountText;
                myItem = value;
                if (transform.childCount != 0)
                    DestroyImmediate(transform.GetChild(0).gameObject); //using DestroyImmediate because destroy doesnt work
                if (value != null)
                {
                    myItem.ItemAmountUpdate += UpdateAmountText;
                    GameObject itemSlot = Instantiate(itemSlotPrefab, transform);
                    amountText = itemSlot.GetComponentInChildren<TextMeshProUGUI>();
                    itemSlot.GetComponent<Image>().sprite = myItem.itemData.itemSprite;
                    if (value.Amount > 1)
                        amountText.text = $"{value.Amount}";
                }
                else
                {
                    if (amountText)
                        amountText.text = null;
                }
            }
        }

        private void Awake() {
            if (amountText == null)
                amountText = GetComponentInChildren<TextMeshProUGUI>();
            if (!playerHand)
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  playerHand = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0);
        }

        public void EquipSlot()
        {
            if (equippedItem)
                equippedItem.SetActive(false);
            if (myItem != null)
                equippedItem = myItem.Equip(playerHand);
        }

        public void RemoveItem()
        {
            Debug.Log("Removing item");
            if (myItem == null) return;
            myItem.Drop();                                                                  
            MyItem = null;
        }

        public void DeleteItem()
        {
            if (myItem == null) return;
            myItem.Drop();
            MyItem = null;
        }

        private void UpdateAmountText(int newAmount)
        {
            if (amountText)
                amountText.text =  myItem.Amount > 1 ? $"{myItem.Amount}" : "" ;
        }

        public void SelectItem()
        {
            GameObject prevHeldItem = null;
            bool pickedUpItem = false;
            if (transform.childCount != 0)
            {
                //try to stack items
                if (heldItem)
                {
                    var movingItem = heldItem.GetComponent<MoveSlot>().movingItem;
                    if (movingItem != null && myItem != null && movingItem.itemData == myItem.itemData) {
                        myItem.Amount += movingItem.Amount;
                        Destroy(heldItem);
                        return;
                    }
                }
                //pickup item
                var itemSlot = transform.GetChild(0).gameObject;
                itemSlot.transform.SetParent(moveParent, true);
                var moveSlot = itemSlot.GetComponent<MoveSlot>();
                moveSlot.followMouse = true;
                moveSlot.movingItem = myItem;
                prevHeldItem = heldItem;
                heldItem = itemSlot.gameObject;
                //update item to null now no items in this slot
                myItem.ItemAmountUpdate -= UpdateAmountText;
                myItem = null;
                if (amountText)
                    amountText = null;
                pickedUpItem = true;
            }
            
            GameObject itemToPlace = null;
            //case 1: there was a held item before pickup that will now be placed
            if (prevHeldItem)
                itemToPlace = prevHeldItem;
            //case 2: there was no pickup but a held item must be placed
            else if (heldItem && !pickedUpItem) {
                itemToPlace = heldItem;
                heldItem = null;
            }
            //place item in slot
            if (itemToPlace)
            {
                itemToPlace.transform.SetParent(transform);
                itemToPlace.transform.localPosition = Vector3.zero;
                var moveSlot = itemToPlace.GetComponent<MoveSlot>();
                moveSlot.followMouse = false;
                if (moveSlot.movingItem == null) return;
                myItem = moveSlot.movingItem;
                myItem.ItemAmountUpdate += UpdateAmountText;
                amountText = itemToPlace.GetComponentInChildren<TextMeshProUGUI>();
                if (myItem.Amount > 1) {
                    amountText.text = $"{myItem.Amount}";
                }
            }
            InventorySlotChanged.Invoke();
        }
    }
}