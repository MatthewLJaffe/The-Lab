using System.Linq;
using PlayerScripts;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace InventoryScripts.ItemScripts
{
    public class ItemSpawner : MonoBehaviour
    {
        public UnityEvent onItemSpawn;
        [SerializeField] private ItemTableEntry[] itemTable;
        private SpriteRenderer _sr;
        [SerializeField] private Color highlightColor;
        private bool _inRange;
        private bool _looted;


        private void Awake()
        {
            _sr = GetComponentInParent<SpriteRenderer>();

            float probSum = itemTable.Sum(ie => ie.prob);
            for (int i = 0; i < itemTable.Length; i++)
                itemTable[i].prob = (itemTable[i].prob / probSum) * 100;
            PlayerInputManager.OnInputDown += DetermineLoot;
        }

        [System.Serializable]
        private struct ItemTableEntry
        {
            public float prob;
            public GameObject drop;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player") || _looted) return;
            _sr.color = highlightColor;
            _inRange = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            _sr.color = Color.white;
            _inRange = false;
        }

        private void DetermineLoot(PlayerInputManager.PlayerInputName iName)
        {
            if (iName != PlayerInputManager.PlayerInputName.Interact || _looted || !_inRange) return;
            float probValue = Random.Range(0f, 100f);
            float currentValue = 0;
            foreach (var entry in itemTable)
            {
                if (probValue < currentValue + entry.prob && probValue > currentValue) {
                    SpawnItem(entry.drop);
                    return;
                }
                currentValue += entry.prob;
            }
            SpawnItem(itemTable[0].drop);
        }

        private void SpawnItem(GameObject drop)
        {
            var dropInstance = Instantiate(drop, transform.position, Quaternion.identity);
            StartCoroutine(dropInstance.GetComponent<ItemPickup>().DropItem());
            onItemSpawn.Invoke();
            _looted = true;
        }
    }
}
