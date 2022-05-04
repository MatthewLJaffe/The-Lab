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
            if (_currDamageTime >= damageTickTime) {
                other.GetComponentInChildren<IDamageable>()?.TakeDamage(damage, Vector2.zero);
                _currDamageTime = 0;
            }
        }
    }
}