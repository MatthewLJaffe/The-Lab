using System.Threading.Tasks;
using UnityEngine;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu(fileName = "TwinBladeEffect", menuName = "Effects/TwinBladeEffect")]
    public class TwinBladeEffect : Effect
    {
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private float effectDuration;
        [SerializeField] private float critChanceIncreasePerStack;
        [SerializeField] private float damageIncreasePerStack;
        [SerializeField] private int maxDamageProcs;
        private float _critIncrease;
        private float _bonusCritPerProc;
        private float _effectStartTime;
        private int _effectProcs;

        protected override void OnEnable()
        {
            _effectProcs = 0;
            base.OnEnable();
        }
        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            _critIncrease = critChanceIncreasePerStack * newStack;
            _bonusCritPerProc = damageIncreasePerStack * newStack;
        }

        public async void ApplyTwinBladeEffect()
        {
            if (stack == 0) return;
            _effectStartTime = Time.time;
            _effectProcs++;
            //every time effect procs increase crit multiplier damage
            if (_effectProcs <= maxDamageProcs)
                playerStats.playerStatsDict[PlayerStats.StatType.CritMultiplier].CurrentValue += _bonusCritPerProc;
            //break out if this isn't the first proc
            if (_effectProcs > 1) return;
            //only increase crit chance once for 5 seconds and reset timer every additional proc
            playerStats.playerStatsDict[PlayerStats.StatType.CritChance].CurrentValue += _critIncrease;
            while (_effectStartTime + effectDuration > Time.time ) await Task.Yield();
            playerStats.playerStatsDict[PlayerStats.StatType.CritChance].CurrentValue -= _critIncrease;
            playerStats.playerStatsDict[PlayerStats.StatType.CritMultiplier].CurrentValue -= _effectProcs * _bonusCritPerProc;
            _effectProcs = 0;
        }
    }
}