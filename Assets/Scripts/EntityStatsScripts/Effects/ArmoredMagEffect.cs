using UnityEngine;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu (fileName = "ArmoredMagEffect", menuName = "Effects/ArmoredMagEffect")]
    public class ArmoredMagEffect : Effect
    {
        [SerializeField] private float reloadIncreasePerStack;
        public float armorHealth;
        [SerializeField] private float armorHealthPerStack;
        [SerializeField] private PlayerStats playerStats;
        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            armorHealth = newStack * armorHealthPerStack;
            playerStats.playerStatsDict[PlayerStats.StatType.ReloadFactor].CurrentValue +=
                (newStack - oldStack) * reloadIncreasePerStack;
        }
    }
}