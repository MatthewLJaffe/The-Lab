using UnityEngine;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu (fileName = "AdrenalineEffect", menuName = "Effects/AdrenalineEffect")]
    public class AdrenalineEffect : Effect
    {
        [SerializeField] private PlayerStats _playerStats;

        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            _playerStats.playerStatsDict[PlayerStats.StatType.DodgeChance].CurrentValue = (1 - 1f / (.15f * newStack + 1)) * 100;
        }
    }
}