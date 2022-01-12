using UnityEngine;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu (fileName = "BladeEffect", menuName = "Effects/BladeEffect")]
    public class BladeEffect : Effect
    {
        [SerializeField] private PlayerStats playerStats;

        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            playerStats.PlayerStatsDict[PlayerStats.StatType.Attack].CurrentValue += newStack - oldStack;
        }
    }
}