using UnityEngine;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu (fileName = "FirstAidGuide", menuName = "Effects/FirstAidGuidEffect")]
    public class FirstAidGuide : Effect
    {
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private float maxRestoreMult;
        [SerializeField] private float restoreMultStep;
        private float _restoreConsumableMult;

        protected override void OnEnable()
        {
            base.OnEnable();
            _restoreConsumableMult = 1;
        }

        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            playerStats.PlayerStatsDict[PlayerStats.StatType.RestoreMultiplier].CurrentValue /= _restoreConsumableMult;
            _restoreConsumableMult = maxRestoreMult * (1 - 1 / (1 + newStack * restoreMultStep));
            playerStats.PlayerStatsDict[PlayerStats.StatType.RestoreMultiplier].CurrentValue *= _restoreConsumableMult;
        }
    }
}