using InventoryScripts;
using InventoryScripts.ItemScripts;
using PlayerScripts;
using UnityEngine;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu (fileName = "CursedInjection", menuName = "Effects/CursedInjectionEffect")]
    public class CursedInjectionEffect : Effect
    {
        [SerializeField] private float regenGainPerStack;
        [SerializeField] private PlayerStats playerStats;
        private RegenStat _regen;

        protected override void OnEnable()
        {
            base.OnEnable();
            RestoreConsumable.restoreItemUsed += TryRemoveCursedInjectionEffect;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            RestoreConsumable.restoreItemUsed -= TryRemoveCursedInjectionEffect;
        }
        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            playerStats.playerStatsDict[PlayerStats.StatType.RegenPerTick].CurrentValue +=
                (newStack - oldStack) * regenGainPerStack;
        }

        private void TryRemoveCursedInjectionEffect(RestoreConsumable consumable)
        {
            if (consumable.restoreType == PlayerBar.PlayerBarType.Health)
                Stack = 0;
        }
    }
}