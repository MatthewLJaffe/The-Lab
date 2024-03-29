﻿using PlayerScripts;
using UnityEngine;
using WeaponScripts;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu (fileName = "CommandosBandannaEffect", menuName = "Effects/CommandosBandannaEffect")]
    public class CommandosBandannaEffect : Effect
    {
        private float _damageMult;
        [SerializeField] private float maxDamageMult;
        [SerializeField] private float damageStep;
        [SerializeField] private float maxEffectRange;

        protected override void OnEnable()
        {
            base.OnEnable();
            _damageMult = 0;
            PlayerBullet.bulletDamage += ApplyCommandosBandannaEffect;
        }

        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            _damageMult = Mathf.Min(maxDamageMult, damageStep * newStack + 1f);
        }

        private void ApplyCommandosBandannaEffect(Bullet b, bool damage)
        {
            if (!damage || stack == 0) return;
            var distance =
                Vector2.Distance(b.transform.position, PlayerFind.instance.playerInstance.transform.position);
            if (stack == 0 || b.LayerInMask(LayerMask.NameToLayer("Player")) ||
                distance > maxEffectRange) return;
                b.damage *= _damageMult;
        }
    }
}
