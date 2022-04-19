using UnityEngine;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu (fileName = "HealthyEffect", menuName = "Effects/HealthyEffect")]
    public class HealthyEffect : Effect
    {
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private float defBonus = 2.5f;
        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            playerStats.playerStatsDict[PlayerStats.StatType.Defense].CurrentValue += defBonus * (newStack - oldStack);
        }
    }
}