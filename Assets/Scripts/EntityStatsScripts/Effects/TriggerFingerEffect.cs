using UnityEngine;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu (fileName = "TriggerFingerEffect", menuName = "Effects/TriggerFingerEffect")]
    public class TriggerFingerEffect : Effect
    {
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private float maxFireRateBonus = 5f;
        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            playerStats.PlayerStatsDict[PlayerStats.StatType.MaxDisease].CurrentValue +=
                100 * (.5f + .5f / (1f + newStack * .5f )) - 100 * (.5f + .5f / (1f + oldStack * .5f ));
            playerStats.PlayerStatsDict[PlayerStats.StatType.FireRate].CurrentValue +=
                ( 1 - 1 / (1 + newStack * .5f ) - (1 - 1 / (1 + oldStack * .5f)) ) * maxFireRateBonus;

        }
    }
}