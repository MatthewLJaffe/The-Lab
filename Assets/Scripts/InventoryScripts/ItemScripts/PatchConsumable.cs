﻿using System.Linq;
using EntityStatsScripts;
using General;
using PlayerScripts;
using UnityEngine;

namespace InventoryScripts.ItemScripts
{
    public class PatchConsumable : RestoreConsumable
    {
        [SerializeField] private float restoreAmount;
        [SerializeField] private float restoreDuration;
        [SerializeField] private GameObject restoreTextPrefab;
        private Transform _displayPoint;

        protected override void Consume(PlayerInputManager.PlayerInputName inputName)
        {
            if (inputName != PlayerInputManager.PlayerInputName.Fire1 || !gameObject.activeSelf) return;
            var regen = transform.parent.parent.gameObject.AddComponent<RegenStat>();
            regen.StartRegen(restoreType, restoreAmount * restoreMultiplier, restoreDuration, restoreTextPrefab);
            base.Consume(inputName);
        }
    }
}