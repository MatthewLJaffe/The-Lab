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
        [SerializeField] private float tickTime;
        private RegenStat _regen;
        private float _regenPerTick;

        protected override void OnEnable()
        {
            base.OnEnable();
            _regenPerTick = 0;
            RestoreConsumable.restoreItemUsed += TryRemoveCursedInjectionEffect;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            RestoreConsumable.restoreItemUsed -= TryRemoveCursedInjectionEffect;
        }
        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            if (!_regen)
                _regen = PlayerFind.instance.playerInstance.AddComponent<RegenStat>();

            _regen.StopRegen();
            if (newStack == 0) {
                _regen.enabled = false;
                return;
            }

            if (!_regen.enabled)
                _regen.enabled = true;
            _regenPerTick = newStack * regenGainPerStack;
            _regen.StartIndefiniteRegen(PlayerBar.PlayerBarType.Health, _regenPerTick, tickTime);
        }

        private void TryRemoveCursedInjectionEffect(RestoreConsumable consumable)
        {
            if (consumable.restoreType == PlayerBar.PlayerBarType.Health)
                Stack = 0;
        }
    }
}