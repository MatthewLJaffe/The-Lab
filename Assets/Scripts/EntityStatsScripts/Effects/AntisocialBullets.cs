using System;
using UnityEngine;
using WeaponScripts;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu (fileName = "AntisocialBulletsEffect", menuName = "Effects/AntisocialBulletsEffect")]
    
    public class AntisocialBullets : Effect
    {
        [SerializeField] private float attackStep;
        [SerializeField] private PlayerStats playerStats;
        private float _attackBonus;
        private bool _inEffect;

        protected override void OnEnable()
        {
            base.OnEnable();
            Gun.broadcastShot += ApplyAntisocialBulletsEffect;
            _attackBonus = 0;
            _inEffect = false;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Gun.broadcastShot -= ApplyAntisocialBulletsEffect;
        }

        private void ApplyAntisocialBulletsEffect(int bulletsLeft, int bulletsInMag)
        {
            if (_inEffect && bulletsLeft != 0)
            {
                playerStats.playerStatsDict[PlayerStats.StatType.Attack].CurrentValue -= _attackBonus;
                _inEffect = false;
            }
                
            if (bulletsLeft != 0 || stack == 0 || _inEffect) return;
            _inEffect = true;
            playerStats.playerStatsDict[PlayerStats.StatType.Attack].CurrentValue += _attackBonus;
        }

        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            _attackBonus = newStack * attackStep;
        }
    }
}