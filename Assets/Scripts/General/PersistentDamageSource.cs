using EntityStatsScripts;
using UnityEngine;

namespace General
{
    public class PersistentDamageSource : DamageSource
    {
        [SerializeField] private float damageTickTime;
        private float _currDamageTime;
        protected void OnTriggerStay2D(Collider2D other)
        {
            if ( !LayerInMask(other.gameObject.layer)) return;
            _currDamageTime += Time.fixedDeltaTime;
            if (_currDamageTime >= damageTickTime)
            {
                var damageable = other.GetComponentInChildren<IDamageable>();
                if (damageable == null) return;
                damageable.TakeDamage(damage, Vector2.zero, this);
                onDamage.Invoke();
                _currDamageTime = 0;
            }
        }
    }
}