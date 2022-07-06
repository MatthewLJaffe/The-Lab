using UnityEngine;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu(fileName = "PHDEffect", menuName = "Effects/PHDEffect")]
    public class PHDEffect : Effect
    {
        [SerializeField] private float defPerStack;
        [SerializeField] private PlayerStats playerStats;
        private float _kitDefBonus;
        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            _kitDefBonus = newStack * defPerStack;
        }
        
        public void ApplyPHDEffect()
        {
            playerStats.playerStatsDict[PlayerStats.StatType.Defense].CurrentValue += _kitDefBonus;
        }
    }
}