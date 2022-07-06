using System.Threading.Tasks;
using UnityEngine;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu(fileName = "LightWeightDaggerEffect", menuName = "Effects/LightweightDaggerEffect")]
    public class LightweightDagger : Effect
    {
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private float fireRateIncreasePerStack;
        [SerializeField] private float speedIncreasePerStack;
        [SerializeField] private float effectDuration;
        private float _fireRateIncrease;
        private float _speedIncrease;
        private float _effectStartTime;
        private bool _effectApplied;
        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            _fireRateIncrease = newStack * fireRateIncreasePerStack;
            _speedIncrease = newStack * speedIncreasePerStack;
            _effectApplied = false;
        }

        public async void ApplyLightWeightDaggerEffect()
        {
            if (stack == 0) return;
            _effectStartTime = Time.time;
            if (_effectApplied) return;
            _effectApplied = true;
            playerStats.playerStatsDict[PlayerStats.StatType.FireRate].CurrentValue += _fireRateIncrease;
            playerStats.playerStatsDict[PlayerStats.StatType.Speed].CurrentValue += _speedIncrease;
            while (_effectStartTime + effectDuration > Time.time ) await Task.Yield();
            playerStats.playerStatsDict[PlayerStats.StatType.FireRate].CurrentValue -= _fireRateIncrease;
            playerStats.playerStatsDict[PlayerStats.StatType.Speed].CurrentValue -= _speedIncrease;
            _effectApplied = false;
        }
    }
}