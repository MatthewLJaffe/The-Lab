using System.Linq;
using UnityEngine;
using WeaponScripts;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu(fileName = "AssasinsHandbookEffect", menuName = "Effects/AssasinsHandbookEffect")]
    public class AssasinsHandbookEffect : Effect
    {
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private AccuracySpeed[] accuracySpeeds;
        public float overallAccuracy;
        private int _totalShots;
        private int _hitShots;
        private float _currSpeedModfier;
        [System.Serializable]
        private struct AccuracySpeed
        {
            public float accuracy;
            public float speed;
        }

        protected override void OnEnable()
        {
            _totalShots = 0;
            _hitShots = 0;
            _currSpeedModfier = 0;
            overallAccuracy = 0;
            base.OnEnable();
            accuracySpeeds = accuracySpeeds.OrderByDescending(ass => ass.accuracy).ToArray();
        }

        protected void ModifySpeed(PlayerBullet b, bool damage)
        {
            _totalShots++;
            if (damage)
                _hitShots++;
            overallAccuracy = _hitShots / (float)_totalShots;
            playerStats.playerStatsDict[PlayerStats.StatType.Speed].CurrentValue -= stack * _currSpeedModfier;
            foreach (var ass in accuracySpeeds)
            {
                if (overallAccuracy >= ass.accuracy)
                {
                    _currSpeedModfier = ass.speed;
                    break;
                }
            }
            playerStats.playerStatsDict[PlayerStats.StatType.Speed].CurrentValue += stack * _currSpeedModfier;
        }

        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            if (newStack == 0)
                PlayerBullet.bulletDamage -= ModifySpeed;
            else if (newStack == 1)
                PlayerBullet.bulletDamage += ModifySpeed;
            playerStats.playerStatsDict[PlayerStats.StatType.Speed].CurrentValue -= oldStack * _currSpeedModfier;
            playerStats.playerStatsDict[PlayerStats.StatType.Speed].CurrentValue += newStack * _currSpeedModfier;
        }
    }
}