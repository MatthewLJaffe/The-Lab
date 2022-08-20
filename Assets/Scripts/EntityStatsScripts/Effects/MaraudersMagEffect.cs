using System.Threading.Tasks;
using EnemyScripts;
using UnityEngine;
using WeaponScripts;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu(fileName = "MaraudersMagEffect", menuName = "Effects/MaraudersMagEffect")]
    public class MaraudersMagEffect : Effect
    {
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private float effectDuration;
        [SerializeField] private float deathWaitTime;
        [SerializeField] private float atkBonusPerStack;
        private float _attackBonus;
        private float _startTime;
        private float _deathStartTime;
        private bool _effectActive;
        private bool _canProcEffect;

        protected override void OnEnable()
        {
            Enemy.broadcastDeath += HandleDeath;
            Gun.broadcastReload += ApplyMaraudersMagEffect;
            _effectActive = false;
            _canProcEffect = false;
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            Enemy.broadcastDeath -= HandleDeath;
            Gun.broadcastReload -= ApplyMaraudersMagEffect;
            base.OnDisable();
        }
        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            _attackBonus = newStack * atkBonusPerStack;
        }

        public async void HandleDeath()
        {
            _deathStartTime = Time.time;
            if (_canProcEffect) return;
            _canProcEffect = true;
            while (_deathStartTime + deathWaitTime > Time.time) await Task.Yield();
            _canProcEffect = false;
        }

        public async void ApplyMaraudersMagEffect(float reloadTime)
        {
            if (!_canProcEffect || stack == 0) return;
            _startTime = Time.time;
            if (_effectActive) return;
            _effectActive = true;
            playerStats.playerStatsDict[PlayerStats.StatType.Attack].CurrentValue += _attackBonus;
            while (effectDuration + _startTime + reloadTime / 2 > Time.time) await Task.Yield();
            playerStats.playerStatsDict[PlayerStats.StatType.Attack].CurrentValue -= _attackBonus;
            _effectActive = false;
        }
    }
}