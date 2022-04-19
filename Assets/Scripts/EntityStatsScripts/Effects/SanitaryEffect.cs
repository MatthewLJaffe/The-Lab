using UnityEngine;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu (fileName = "SanitaryEffect", menuName = "Effects/SanitaryEffect")]
    public class SanitaryEffect : Effect
    {
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private float healthBonus = 25f;

        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            playerStats.playerStatsDict[PlayerStats.StatType.MaxHealth].CurrentValue += healthBonus * (newStack - oldStack);

        }
    }
}