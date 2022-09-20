using System;
using System.Threading.Tasks;
using UnityEngine;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu (fileName = "SteroidEffect", menuName = "Effects/SteroidEffect")]
    public class SteroidEffect : Effect
    {
        public Action steroidEffectStart = delegate {  };
        public Action steroidEffectEnd = delegate {  };
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private float attackPerStack;
        [SerializeField] private float baseDuration;
        [SerializeField] private float durationPerStack;
        public float attackBounus;
        public float duration;
        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            attackBounus = newStack * attackPerStack;
            duration = baseDuration + (newStack - 1) * durationPerStack;
        }

        public async void ApplySteroidEffect()
        {
            if (stack == 0) return;
            steroidEffectStart.Invoke();
            playerStats.playerStatsDict[PlayerStats.StatType.Attack].CurrentValue += attackBounus;
            var end = Time.time + duration;
            while (end > Time.time)
                await Task.Yield();
            playerStats.playerStatsDict[PlayerStats.StatType.Attack].CurrentValue -= attackBounus;
            steroidEffectEnd.Invoke();
        }
    }
}