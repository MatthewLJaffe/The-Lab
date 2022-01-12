using System;
using InventoryScripts.ItemScripts;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InventoryScripts
{
    public class DragSlot : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private Transform moveParent;
        private Transform _inventoryPanel;
        private InventorySlot _inventorySlot;
        public static Action<Transform, Transform> MoveItems = delegate { _itemsCheckedForSwap = false; };
        private static Action<Transform, Transform> _swapItems = delegate { };
        private bool _dragging;
        private bool _enter;
        private static bool _itemsCheckedForSwap;
        private static Transform _swapTo;
        private Transform _swapFrom;

        private void Awake() {
            _inventoryPanel = transform.parent.parent;
            _inventorySlot = GetComponent<InventorySlot>();
            MoveItems += OnMoveItems;
            _swapItems += OnSwapItems;
            Trash.TrashItem += DeleteItem;
        }

        private void OnDestroy()
        {
            MoveItems -= OnMoveItems;
            _swapItems -= OnSwapItems;
            Trash.TrashItem -= DeleteItem;
        }

        private void Update() 
        {
            if (Vector2.Distance(Input.mousePosition, transform.position) < 8 && !_enter)
                PointerEnter();
            else if (Vector2.Distance(Input.mousePosition, transform.position) >= 8 && _enter)
                PointerExit();
            if (_dragging)
                PointerStay();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_inventorySlot.MyItem == null)
                return;
            Debug.Log(transform.parent.gameObject);
            _dragging = true;
            _swapFrom = transform.parent;
            moveParent.position = Input.mousePosition;
            transform.SetParent(moveParent);
            _swapTo = null;
        }

        private void PointerEnter()
        {
            if (!_dragging)
                _swapTo = transform.parent;
            _enter = true;
        }

        private void PointerExit()
        {
            if (_swapTo == transform.parent)
                _swapTo = null;
            _enter = false;
        }
        private void PointerStay()
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (!_swapTo)
                {
                    if (!InsideInventory())
                        GetComponent<InventorySlot>().RemoveItem();
                    MoveTo(_swapFrom);
                }
                MoveItems.Invoke(_swapFrom, _swapTo);
                _dragging = false;
                _swapFrom = null;
                return;
            }
            transform.position = Input.mousePosition;
        }

        private void MoveTo(Transform dest)
        {
            if (dest == null) return;
            transform.SetParent(dest);
            transform.localPosition = Vector3.zero;
        }
        
        //TODO check to see if items contain same item data and don't swap them if they do
        private void OnMoveItems(Transform moveFrom, Transform moveTo)
        {
            if (_itemsCheckedForSwap || !moveTo) return;
            _itemsCheckedForSwap = true;
            var moveFromSlot = moveFrom.GetComponentInChildren<InventorySlot>();
            if (moveFromSlot == null)
                moveFromSlot = moveParent.GetComponentInChildren<InventorySlot>();
            var moveToSlot = moveTo.GetComponentInChildren<InventorySlot>();
            if (moveFromSlot.MyItem != null && moveToSlot.MyItem != null &&
                moveFromSlot.MyItem.itemData == moveToSlot.MyItem.itemData)
            {
                Inventory.Instance.AddItem(new Item(moveFromSlot.MyItem.itemData, moveFromSlot.MyItem.Amount));
                moveFromSlot.MyItem = null;
                moveFromSlot.transform.SetParent(moveFrom);
                moveFromSlot.transform.localPosition = Vector3.zero;
            }
            else
                _swapItems.Invoke(moveFrom, moveTo);
        }

        private void OnSwapItems(Transform swapFrom, Transform swapTo)
        {
            if (swapFrom == transform.parent || transform.parent == moveParent)
            {
                MoveTo(swapTo);
            }
            else if (swapTo == transform.parent)
                 MoveTo(swapFrom);
        }

        private bool InsideInventory()
        {
            var rectTransform = _inventoryPanel as RectTransform;
            Vector2 localMousePosition =  _inventoryPanel.InverseTransformPoint(Input.mousePosition);
            return rectTransform && rectTransform.rect.Contains(localMousePosition);
        }

        private void DeleteItem() 
        {
            if (_swapFrom == transform.parent) {
                GetComponent<InventorySlot>().MyItem = null;
                _dragging = false;
            }
        }
    }
}
