using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InventoryScripts.ItemScripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    
    public class ItemPickup : MonoBehaviour
    {
        [SerializeField] private ItemData itemData;
        public Item itemToDrop;
        public static Action<Item> pickup = delegate{ };
        private Rigidbody2D _rb;
        private bool _canDrop = true;

        private void Awake() {
            _rb = GetComponent<Rigidbody2D>();
            if (itemToDrop == null)
                itemToDrop = new Item(itemData);
        }

        private void OnEnable()
        {
            _canDrop = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryPickupItem(other.gameObject);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            TryPickupItem(other.gameObject);
        }

        private void TryPickupItem(GameObject other)
        {
            if (other.CompareTag("Player") && _canDrop)
            {
                _canDrop = false;
                pickup.Invoke(itemToDrop);
                gameObject.SetActive(false);
            }
        }

        public void TryPickupItem()
        {
            if (!_canDrop) return;
            _canDrop = false;
            if (itemToDrop == null)
                itemToDrop = new Item(itemData);
            pickup.Invoke(itemToDrop);
            gameObject.SetActive(false);
        }

        public IEnumerator DropItem()
        {
            _rb.AddForce(new Vector2(Random.Range(-.8f,.8f), Random.Range(-.2f,-1f)).normalized * 250f);
            _canDrop = false;
            yield return new WaitForSeconds(.5f);
            _canDrop = true;
        }
    }
}