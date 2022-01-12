using UnityEngine;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu (fileName = "CorruptionEffect", menuName = "Effects/CorruptionEffect")]
    public class CorruptionEffect : Effect
    {
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private float attackBonus = 2.5f;
        [SerializeField] private float speedBonus = 2f;

        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            playerStats.PlayerStatsDict[PlayerStats.StatType.Attack].CurrentValue += attackBonus * (newStack - oldStack);
            playerStats.PlayerStatsDict[PlayerStats.StatType.Attack].CurrentValue += speedBonus * (newStack - oldStack);
        }
    }
}