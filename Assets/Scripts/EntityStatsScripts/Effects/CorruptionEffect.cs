using UnityEngine;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu (fileName = "CorruptionEffect", menuName = "Effects/CorruptionEffect")]
    public class CorruptionEffect : Effect
    {
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private float attackBonus;
        [SerializeField] private float speedBonus;

        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            playerStats.playerStatsDict[PlayerStats.StatType.Attack].CurrentValue += attackBonus * (newStack - oldStack);
            playerStats.playerStatsDict[PlayerStats.StatType.Speed].CurrentValue += speedBonus * (newStack - oldStack);
        }
    }
}