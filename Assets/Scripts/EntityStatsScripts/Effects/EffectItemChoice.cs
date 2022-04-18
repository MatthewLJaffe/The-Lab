using System;
using System.Collections.Generic;
using InventoryScripts.ItemScripts;
using PlayerScripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EntityStatsScripts.Effects
{
    public class EffectItemChoice : MonoBehaviour
    {
        [SerializeField] private List<GameObject> effectItems;
        [SerializeField] private Transform[] dropPositions;
        [SerializeField] private Color highlightColor;
        private int _itemToSelect = -1;
        public int ItemToSelect
        {
            get => _itemToSelect;
            set
            {
                if (value == -1 && _itemToSelect != -1)
                {
                    var sr = transform.GetChild(_itemToSelect).GetComponentInChildren<SpriteRenderer>();
                    if (sr == null) return;
                    sr.color = Color.white;
                }
                if (value != -1 && value < transform.childCount)
                {
                    transform.GetChild(value).GetComponentInChildren<SpriteRenderer>().color = highlightColor;
                }
                _itemToSelect = value;
            }
        }

        private void Awake()
        {
            PlayerInputManager.OnInputDown += SelectEffectItem;
            foreach (var trans in dropPositions)
            {
                var dropPrefab = effectItems[Random.Range(0, effectItems.Count)];
                if( !effectItems.Remove(dropPrefab) )
                    Debug.LogError("error removing " + dropPrefab.name);
                var dropInstance = Instantiate(dropPrefab, trans);
                dropInstance.transform.localPosition = Vector3.zero;
                dropInstance.layer = LayerMask.NameToLayer("Invincible");
            }
        }

        private void OnDestroy()
        {
            PlayerInputManager.OnInputDown -= SelectEffectItem;
        }

        private void SelectEffectItem(PlayerInputManager.PlayerInputName iName)
        {
            if (iName != PlayerInputManager.PlayerInputName.Interact || 
                _itemToSelect == -1 || _itemToSelect >= transform.childCount) 
                return;
            transform.GetChild(_itemToSelect).GetComponentInChildren<ItemPickup>().TryPickupItem();
            for (var c = 0; c < transform.childCount; c++)
                transform.GetChild(c).gameObject.SetActive(false);
            _itemToSelect = -1;
        }
    }
}