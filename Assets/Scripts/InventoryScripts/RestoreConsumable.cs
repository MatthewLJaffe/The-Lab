using System;
using EntityStatsScripts;
using EntityStatsScripts.Effects;
using PlayerScripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InventoryScripts
{
    public class RestoreConsumable : Consumable
    {
        protected float restoreMultiplier;
        private float _dontConsumeChance;
        [SerializeField] protected PlayerStats playerStats;
        [SerializeField] protected SteroidEffect steroidEffect;
        [SerializeField] private PHDEffect phdEffect;
        public PlayerBar.PlayerBarType restoreType;
        public static Action<RestoreConsumable> restoreItemUsed = delegate {  };


        protected override void Awake()
        {
            base.Awake();
            restoreMultiplier = playerStats.playerStatsDict[PlayerStats.StatType.RestoreMultiplier].CurrentValue;
            _dontConsumeChance = playerStats.playerStatsDict[PlayerStats.StatType.DontConsumeChance].CurrentValue;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            PlayerStats.onStatChange -= ChangeRestoreMultiplier;
        }

        private void ChangeRestoreMultiplier(PlayerStats.StatType stat, float value)
        {
            switch (stat)
            {
                case PlayerStats.StatType.RestoreMultiplier:
                    restoreMultiplier = value;
                    break;
                case PlayerStats.StatType.DontConsumeChance:
                    _dontConsumeChance = value;
                    break;
            }
        }

        protected override void Consume(PlayerInputManager.PlayerInputName inputName)
        {
            if (inputName != PlayerInputManager.PlayerInputName.Fire1 || !gameObject.activeSelf) return;
            restoreItemUsed.Invoke(this);
            onUse.Invoke();
            if (Random.Range(0f, 1f) > _dontConsumeChance)
                itemConsumed.Invoke();
            if (steroidEffect.Stack > 0)
            {
                steroidEffect.ApplySteroidEffect();
            }
            if (phdEffect && phdEffect.Stack > 0)
                phdEffect.ApplyPHDEffect();
        }
    }
}