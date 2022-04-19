using UnityEngine;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu (fileName = "SurvivalInstinctEffect", menuName = "Effects/SurvivalInstinctEffect")]
    public class SurvivalInstinctEffect : Effect
    {
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private float accuracyBonus = 5f;
        [SerializeField] private float critBonus = 5f;

        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            playerStats.playerStatsDict[PlayerStats.StatType.Accuracy].CurrentValue += accuracyBonus * (newStack - oldStack);
            playerStats.playerStatsDict[PlayerStats.StatType.CritChance].CurrentValue += critBonus * (newStack - oldStack);
        }
    }
}