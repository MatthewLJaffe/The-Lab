using General;
using UnityEngine;

namespace EntityStatsScripts
{
    public interface IDamageable
    {
        void TakeDamage(float amount, Vector2 dir, DamageSource source = null, bool crit = false);
    }
}
