using System;
using EntityStatsScripts.Effects;
using UnityEngine;

namespace EntityStatsScripts
{
    [Serializable]
    public class BonusValue
    {
        /*
        public PlayerStats.StatType bonusType;
        [SerializeField] private float maxBonus;
        [SerializeField] private float currentBonus;
        [SerializeField] private Effect myEffect;

        public float Bonus
        {
            get => currentBonus;
            private set => currentBonus = Mathf.Clamp(value, 0, maxBonus);
        }

        public void SetPlayerBonus(PlayerStats ps, float factor, EffectManager effectManager)
        {
            var newBonusValue = maxBonus * factor;
            ps.PlayerStatsDict[bonusType].CurrentValue += newBonusValue - currentBonus;
            Bonus = newBonusValue;
            if (Bonus > 0)
            {
                myEffect.CurrentMessage = myEffect.message + "\ncurrent +" + Bonus;
                effectManager.AddEffect(myEffect);
            }
            else if (Bonus == 0)
                effectManager.RemoveEffect(myEffect);
        }
        */
    }
}