using System;
using EnemyScripts;
using UnityEngine;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu (fileName = "CheapSkatesFirstAidGuide", menuName = "Effects/CheapSkatesFirstAidGuidEffect")]
    public class CheapSkatesFirstAidGuide : Effect
    {
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private float minRestoreMult;
        [SerializeField] private float dontConsumeStep;
        [SerializeField] private float restoreMultStep;
        private float _dontConsumeChance;
        private float _restoreConsumableMult;

        protected override void OnEnable()
        {
            base.OnEnable();
            _dontConsumeChance = 0;
            _restoreConsumableMult = 1;
        }

        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            if (_dontConsumeChance > .01f && playerStats.PlayerStatsDict[PlayerStats.StatType.DontConsumeChance].CurrentValue > .01f)
            {
                playerStats.PlayerStatsDict[PlayerStats.StatType.DontConsumeChance].CurrentValue /= _dontConsumeChance;
                _dontConsumeChance = 1 - 1 / (1 + newStack * dontConsumeStep);
                playerStats.PlayerStatsDict[PlayerStats.StatType.DontConsumeChance].CurrentValue *= _dontConsumeChance;
            }
            else
            {
                _dontConsumeChance = 1 - 1 / (1 + newStack * dontConsumeStep);
                playerStats.PlayerStatsDict[PlayerStats.StatType.DontConsumeChance].CurrentValue = _dontConsumeChance;
            }

            
            playerStats.PlayerStatsDict[PlayerStats.StatType.RestoreMultiplier].CurrentValue /= _restoreConsumableMult;
            _restoreConsumableMult = minRestoreMult + (1 - minRestoreMult) * 1 / (1 + newStack * restoreMultStep);
            playerStats.PlayerStatsDict[PlayerStats.StatType.RestoreMultiplier].CurrentValue *= _restoreConsumableMult;
        }
    }
}