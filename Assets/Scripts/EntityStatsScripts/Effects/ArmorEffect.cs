using UnityEngine;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu (fileName = "ArmorEffect", menuName = "Effects/ArmorEffect")]
    public class ArmorEffect : Effect
    {
        [SerializeField] private PlayerStats playerStats;

        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            playerStats.PlayerStatsDict[PlayerStats.StatType.Defense].CurrentValue += newStack - oldStack;
        }
    }
}