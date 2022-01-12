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
        public Item ItemToDrop;
        public static Action<Item> Pickup = delegate{ };
        private Collider2D _collider2D;
        private Rigidbody2D _rb;
        private bool _canDrop = true;

        private void Awake() {
            _collider2D = GetComponent<Collider2D>();
            _rb = GetComponent<Rigidbody2D>();
            if (ItemToDrop == null)
                ItemToDrop = new Item(itemData);
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
                Pickup.Invoke(ItemToDrop);
                gameObject.SetActive(false);
            }
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