using System;
using System.Collections;
using General;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InventoryScripts.ItemScripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    
    public class ItemPickup : MonoBehaviour
    {
        [SerializeField] private ItemData itemData;
        [SerializeField] private SoundEffect pickUpSound;
        [SerializeField] private TextMeshProUGUI tmp;
        public Item itemToDrop;
        public static Action<Item> pickup = delegate{ };
        private Rigidbody2D _rb;
        private bool _canDrop = true;

        private void Awake() {
            _rb = GetComponent<Rigidbody2D>();
            if (itemToDrop == null)
                itemToDrop = new Item(itemData);
            tmp.text = itemData.itemDescription;
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
                pickUpSound.Play();
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

        public IEnumerator DropItem(Vector2 spawnDir)
        {
            _rb.AddForce((spawnDir.normalized + new Vector2(Random.Range(-1f,1f), Random.Range(-1f, 1f)).normalized * .2f) * 250f);
            _canDrop = false;
            yield return new WaitForSeconds(.5f);
            _canDrop = true;
        }
    }
}