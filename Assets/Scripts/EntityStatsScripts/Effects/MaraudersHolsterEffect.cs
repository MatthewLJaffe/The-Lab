using System;
using System.Threading.Tasks;
using UnityEngine;
using WeaponScripts;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu (fileName = "MaraudersHolsterEffect", menuName = "Effects/MaraudersHolsterEffect")]
    public class MaraudersHolsterEffect : Effect
    {
        [SerializeField] private float damageBuffTime;
        [SerializeField] private float switchWeaponCooldown;
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private float attackStep;
        private float _attackBonus;
        private bool _inSwitchCooldown;

        protected override void OnEnable()
        {
            base.OnEnable();
            Gun.broadCastWeaponSwitch += ApplyMaraudersHolsterEffect;
            _inSwitchCooldown = false;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Gun.broadCastWeaponSwitch -= ApplyMaraudersHolsterEffect;
        }

        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            _attackBonus = attackStep * newStack;
        }

        private async void ApplyMaraudersHolsterEffect(Gun switchTo)
        {
            if (_inSwitchCooldown || stack == 0) return;
            StartSwitchWeaponCooldown();
            playerStats.playerStatsDict[PlayerStats.StatType.Attack].CurrentValue += _attackBonus;
            var startTime = Time.time;
            while (startTime + damageBuffTime > Time.time)
                await Task.Yield();
            playerStats.playerStatsDict[PlayerStats.StatType.Attack].CurrentValue -= _attackBonus;
        }

        private async void StartSwitchWeaponCooldown()
        {
            _inSwitchCooldown = true;
            var startTime = Time.time;
            while (startTime + switchWeaponCooldown > Time.time)
                await Task.Yield();
            _inSwitchCooldown = false;
        }
    }
}