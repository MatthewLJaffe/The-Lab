using System.Linq;
using InventoryScripts;
using UnityEngine;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu(fileName = "HoardersSeason3", menuName = "Effects/HoardersSeason3")]
    public class HoardersSeason3 : Effect
    {
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private float atkPerItem;
        private float _atkMod;
        protected override void OnEnable()
        {
            _atkMod = 0;
            Inventory.OnInventoryUpdated += ModifyAttack;
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Inventory.OnInventoryUpdated -= ModifyAttack;
        }
        

        protected override void ChangeEffectStack(int newStack, int oldStack) { }

        private void ModifyAttack()
        {
            if (stack == 0) return;
            playerStats.playerStatsDict[PlayerStats.StatType.Attack].CurrentValue -= _atkMod;
            _atkMod = atkPerItem * Inventory.Instance.itemList.Sum(i => i.Amount) * stack;
            playerStats.playerStatsDict[PlayerStats.StatType.Attack].CurrentValue += _atkMod;
        }
    }
}