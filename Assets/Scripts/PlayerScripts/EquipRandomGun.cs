using System;
using InventoryScripts.ItemScripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlayerScripts
{
    public class EquipRandomGun : MonoBehaviour
    {
        [SerializeField] private GameObject[] startWeapons;

        private void Start()
        {
            Instantiate(startWeapons[Random.Range(0, startWeapons.Length)]).
                GetComponent<ItemPickup>().TryPickupItem();
        }
    }
}