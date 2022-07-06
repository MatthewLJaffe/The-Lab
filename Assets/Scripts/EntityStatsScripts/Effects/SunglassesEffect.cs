using System.Threading.Tasks;
using PlayerScripts;
using UnityEngine;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu(fileName = "SunglassesEffect", menuName = "Effects/SunglassesEffect")]
    public class SunglassesEffect : Effect
    {
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private float accuracyPerStack;
        [SerializeField] private float baseDuration;
        [SerializeField] private float additionalDurationPerStack;
        private float _buffDuration;
        private float _accuracyBuff;
        private float _buffStartTime;
        private bool _buffActive;

        protected override void OnEnable()
        {
            PlayerRoll.onRoll += ApplyAccuracyBuff;
            _buffActive = false;
            _buffStartTime = 0;
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            PlayerRoll.onRoll -= ApplyAccuracyBuff;
            base.OnDisable();
        }
        
        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            _accuracyBuff = newStack * accuracyPerStack;
            _buffDuration = (newStack - 1) * additionalDurationPerStack + baseDuration;
        }

        private async void ApplyAccuracyBuff(bool rolling, Vector2 rollDir)
        {
            if (!rolling || stack == 0) return;
            _buffStartTime = Time.time;
            if (_buffActive) return;
            _buffActive = true;
            playerStats.playerStatsDict[PlayerStats.StatType.Accuracy].CurrentValue += _accuracyBuff;
            while (Time.time < _buffStartTime + _buffDuration) await Task.Yield();
            playerStats.playerStatsDict[PlayerStats.StatType.Accuracy].CurrentValue -= _accuracyBuff;
            _buffActive = false;
        }
    }
}