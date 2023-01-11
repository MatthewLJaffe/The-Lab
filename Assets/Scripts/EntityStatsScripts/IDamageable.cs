using General;
using UnityEngine;

namespace EntityStatsScripts
{
    /// <summary>
    /// allows damage sources to interface with things that take damage
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(float amount, Vector2 dir, DamageSource source = null, bool crit = false);
    }
}
