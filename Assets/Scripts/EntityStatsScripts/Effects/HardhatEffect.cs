using System;
using System.Threading.Tasks;
using General;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu(fileName = "HardhatEffect", menuName = "Effects/HardhatEffect")]
    public class HardhatEffect : Effect
    {
        [SerializeField] private GameObject cone;
        [SerializeField] private SoundEffect effectSound;
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private float defenseProbStep;
        [SerializeField] private int[] healthBonuses;
        [SerializeField] private float effectDelay;
        private float _procChance;
        private int _healthIncrease;
        
        

        private async void RollDefense(Vector3 diePos)
        {
            if (Random.Range(0f, 1f) < _procChance) return;
            var t = Time.time;
            while (t + effectDelay > Time.time) await Task.Yield();
            playerStats.playerStatsDict[PlayerStats.StatType.MaxHealth].CurrentValue += _healthIncrease;
            effectSound.Play();
            Instantiate(cone, diePos, Quaternion.identity);
        }


        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            if (newStack == 0)
                EnemyHealth.killedByHazard -= RollDefense;
            if (newStack == 1)
                EnemyHealth.killedByHazard += RollDefense;
            _procChance=  1 - 1f / (defenseProbStep * newStack + 1);
            var hpidx = Math.Min(healthBonuses.Length, newStack - 1);
            _healthIncrease = healthBonuses[hpidx];
        }
        
    }
}