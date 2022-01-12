using System.Collections;
using EntityStatsScripts.Effects;
using UnityEngine;

namespace EntityStatsScripts
{
    public class PlayerInfectionBar : PlayerBar
    {
        [SerializeField] private float baseInfectionRate;
        [SerializeField] private float timeUntilDiseaseTolerance = 100;
        private float _currToleranceTime;
        private float _diseasePerSec;
        private Coroutine _diseaseToleranceRoutine;
        [SerializeField] private DiseaseToleranceEffect diseaseToleranceEffect;
        private bool _ticking;

        public override float BarValue
        {
            get => barValue;
            set
            {
                base.BarValue = value;
                var precentage = barValue / maxValue;
                if (_diseaseToleranceRoutine != null)
                    StopCoroutine(_diseaseToleranceRoutine);
                if (precentage <= .25f)
                    _diseaseToleranceRoutine= StartCoroutine(CountToDiseaseTolerance(2));
                else if (precentage < .5f)
                    _diseaseToleranceRoutine = StartCoroutine(CountToDiseaseTolerance(1));
            }
        }

        private void Awake()
        {
            diseaseToleranceEffect.OnDiseaseToleranceChange += ModifyInfectionRate;
            _diseasePerSec = baseInfectionRate;
            PlayerStats.OnStatChange += ChangeMaxDisease;
        }

        private void OnDestroy()
        {
            diseaseToleranceEffect.OnDiseaseToleranceChange -= ModifyInfectionRate;
            PlayerStats.OnStatChange -= ChangeMaxDisease;
        }

        private void Update()
        {
            if (!_ticking)
                StartCoroutine(TickInfection());
        }

        private void ModifyInfectionRate(float factor)
        {
            _diseasePerSec = baseInfectionRate * factor;
        }

        private IEnumerator TickInfection()
        {
            _ticking = true;
            yield return new WaitForSeconds(1);
            BarValue -= _diseasePerSec;
            _ticking = false;
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