using System;
using EnemyScripts;
using EntityStatsScripts;
using EntityStatsScripts.Effects;
using General;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WeaponScripts
{
    public class PlayerBullet : PooledBullet
    {
        public static Action<PlayerBullet, bool> bulletDamage = delegate { };
        [SerializeField] private SoundEffect critSound;
        [SerializeField] private LightweightDagger lightWeightDaggerEffect;
        [SerializeField] private TwinBladeEffect twinBladeEffect;
        [SerializeField] private VampireFangsEffect vampireFangsEffect;

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject == gameObject || ! sourceCollider.enabled || other.isTrigger) return;
            var damageable = false;
            if (sourceCollider.isTrigger && direction != Vector2.zero &&
                layers == (layers | (1 << other.gameObject.layer)))
            {
                damageable = other.GetComponentInChildren<IDamageable>() != null;
                if (damageable && other.transform.parent && other.transform.parent.GetComponentInChildren<Enemy>())
                    vampireFangsEffect.RollLifesteal(damage);
                if (crit && damageable) {
                    lightWeightDaggerEffect.ApplyLightWeightDaggerEffect();
                    twinBladeEffect.ApplyTwinBladeEffect();
                    critSound.Play();
                }
            }
            bulletDamage.Invoke(this, damageable);
            base.OnTriggerEnter2D(other);
        }

        public void RemoveGlitchBullet()
        {
            if (Random.Range(0f, 1f) > .75f)
                Destroy(gameObject);
        }
    }
    
}