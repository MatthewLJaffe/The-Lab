using System.Collections;
using EntityStatsScripts.Effects;
using PlayerScripts;
using UnityEngine;

namespace EntityStatsScripts
{
    public class PlayerInfectionBar : PlayerBar
    {
        [SerializeField] private float baseInfectionRate;
        [SerializeField] private float timeUntilDiseaseTolerance = 100;
        private float _currToleranceTime;
        private float _diseasePerTick;
        private Coroutine _diseaseToleranceRoutine;
        [SerializeField] private DiseaseToleranceEffect diseaseToleranceEffect;
        private bool _ticking;

        public override float BarValue
        {
            get => barValue;
            set
            {
                base.BarValue = value;
                if (_diseaseToleranceRoutine != null)
                    StopCoroutine(_diseaseToleranceRoutine);
                if (barValue <= barVeryLowValue)
                    _diseaseToleranceRoutine= StartCoroutine(CountToDiseaseTolerance(2));
                else if (barValue < barVeryLowValue)
                    _diseaseToleranceRoutine = StartCoroutine(CountToDiseaseTolerance(1));
            }
        }

        private void Awake()
        {
            diseaseToleranceEffect.OnDiseaseToleranceChange += ModifyInfectionRate;
            _diseasePerTick = baseInfectionRate;
            PlayerStats.onStatChange += ChangeMaxDisease;
            PlayerMove.moveTick += TickDisease;
        }

        private void OnDestroy()
        {
            diseaseToleranceEffect.OnDiseaseToleranceChange -= ModifyInfectionRate;
            PlayerStats.onStatChange -= ChangeMaxDisease;
            PlayerMove.moveTick -= TickDisease;
        }

        private void TickDisease()
        {
            BarValue -= _diseasePerTick;
        }

        private void ModifyInfectionRate(float factor)
        {
            _diseasePerTick = baseInfectionRate * factor;
        }

        private IEnumerator CountToDiseaseTolerance(float factor)
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                _currToleranceTime += factor;
                if (_currToleranceTime <= timeUntilDiseaseTolerance) continue;
                _currToleranceTime = 0;
                diseaseToleranceEffect.Stack++;
            }
        }

        private void ChangeMaxDisease(PlayerStats.StatType type, float newValue)
        {
            if (type == PlayerStats.StatType.MaxDisease) {
                MaxValue = newValue;
            }
        }
    }
}