using System.Linq;
using PlayerScripts;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace InventoryScripts.ItemScripts
{
    public class ItemSpawner : MonoBehaviour, IInteractable
    {
        public UnityEvent onItemSpawn;
        [SerializeField] private ItemTableEntry[] itemTable;
        private SpriteRenderer _sr;
        [SerializeField] private Color highlightColor;
        private bool _looted;
        public bool CanInteract
        {
            set
            {
                if (value && !_looted)
                    _sr.color = highlightColor;
                else
                    _sr.color = Color.white;
            }
        }
        
        public void Interact()
        {
            DetermineLoot();
        }

        private void Awake()
        {
            _sr = GetComponentInParent<SpriteRenderer>();

            float probSum = itemTable.Sum(ie => ie.prob);
            for (int i = 0; i < itemTable.Length; i++)
                itemTable[i].prob = (itemTable[i].prob / probSum) * 100;
        }

        [System.Serializable]
        private struct ItemTableEntry
        {
            public float prob;
            public GameObject drop;
        }

        public void DetermineLoot()
        {
            if (_looted) return;
            var probValue = Random.Range(0f, 100f);
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
            _looted = true;
            if (!drop)  return;
            var dropInstance = Instantiate(drop, transform.position, Quaternion.identity);
            StartCoroutine(dropInstance.GetComponent<ItemPickup>().DropItem());
            onItemSpawn.Invoke();
            _sr.color = Color.white;
        }
    }
}
