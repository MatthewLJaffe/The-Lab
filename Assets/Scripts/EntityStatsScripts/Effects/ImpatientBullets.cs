using System;
using UnityEngine;
using WeaponScripts;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu (fileName = "ImpatientBulletsEffect", menuName = "Effects/ImpatientBulletsEffect")]
    public class ImpatientBullets : Effect
    {
        [SerializeField] private float buffStep;
        [SerializeField] private float debuffStep;
        [SerializeField] private PlayerStats playerStats;
        private float _attackBonus;
        private float _attackPenalty;
        private BuffStatus _status;

        private enum BuffStatus
        {
            Neutral,
            InBuff,
            InDebuff
        };
        
        protected override void OnEnable()
        {
            base.OnEnable();
            Gun.broadcastShot += ApplyImpatientBulletsEffect;
            _attackBonus = 0;
            _attackPenalty = 0;
            _status = BuffStatus.Neutral;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Gun.broadcastShot -= ApplyImpatientBulletsEffect;
        }

        private void ApplyImpatientBulletsEffect(int bulletsLeft, int bulletsInMag)
        {
            if (stack == 0) return;
            switch (_status)
            {
                case BuffStatus.Neutral:
                    if (bulletsLeft >= bulletsInMag / 2) {
                        playerStats.playerStatsDict[PlayerStats.StatType.Attack].CurrentValue += _attackBonus;
                        _status = BuffStatus.InBuff;
                    }
                    else {
                        playerStats.playerStatsDict[PlayerStats.StatType.Attack].CurrentValue -= _attackPenalty;
                        _status = BuffStatus.InDebuff;
                    }
                    break;
                
                case BuffStatus.InBuff:
                    if (bulletsLeft < bulletsInMag / 2)
                    {
                        playerStats.playerStatsDict[PlayerStats.StatType.Attack].CurrentValue -=  _attackBonus + _attackPenalty;
                        _status = BuffStatus.InDebuff;
                    }
                    break;
                
                case BuffStatus.InDebuff:
                    if (bulletsLeft >= bulletsInMag / 2)
                    {
                        playerStats.playerStatsDict[PlayerStats.StatType.Attack].CurrentValue += _attackBonus + _attackPenalty;
                        _status = BuffStatus.InBuff;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            _attackBonus = buffStep * newStack;
            _attackPenalty = debuffStep * newStack;
        }
    }
}