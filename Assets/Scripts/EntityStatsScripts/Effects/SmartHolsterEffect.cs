using System.Threading.Tasks;
using UnityEngine;
using WeaponScripts;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu (fileName = "SmartHolsterEffect", menuName = "Effects/SmartHolsterEffect")]
    public class SmartHolsterEffect : Effect
    {
        [SerializeField] private float switchWeaponCooldown;
        [SerializeField] private float maxReloadMult;
        [SerializeField] private float reloadStep;
        private bool _inSwitchCooldown;

        protected override void OnEnable()
        {
            base.OnEnable();
            Gun.broadCastWeaponSwitch += ApplySmartHolsterEffect;
            _inSwitchCooldown = false;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Gun.broadCastWeaponSwitch -= ApplySmartHolsterEffect;
        }

        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            if (newStack == 0) return;
            switchWeaponCooldown = maxReloadMult * ( 1 / ( 1 + reloadStep * (newStack - 1) ) );
        }

        private void ApplySmartHolsterEffect(Gun switchTo)
        {
            if (_inSwitchCooldown || stack == 0) return;
            StartSwitchWeaponCooldown();
            switchTo.CurrentMagSize = switchTo.gunStats.magSize;
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