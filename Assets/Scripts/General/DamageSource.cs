using EntityStatsScripts;
using UnityEngine;
using UnityEngine.Events;

namespace General
{
    /// <summary>
    /// component that allows objects to interface with damageable components
    /// base class for all sources of damage in the game
    /// </summary>
    public class DamageSource : MonoBehaviour
    {
        public float damage;
        [SerializeField] protected Collider2D sourceCollider;
        public bool isHazard;
        [SerializeField] protected SoundEffect damageSfx;
        public LayerMask layers;
        public UnityEvent onDamage;
        protected virtual void Awake()
        {
            if (!sourceCollider)
                sourceCollider = GetComponent<Collider2D>();
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            //check if layer is in layer mask
            if ( !LayerInMask(collision.gameObject.layer)) return;
            var damageable = collision.GetComponentInChildren<IDamageable>();
            if (damageable == null) return;
            damageable.TakeDamage(damage, Vector2.zero, this);
            onDamage.Invoke();
            if (damageSfx)
                damageSfx.Play();
        }

        protected virtual void OnCollisionEnter2D(Collision2D other)
        {
            //check if layer is in layer mask
            if ( !LayerInMask(other.collider.gameObject.layer)) return;
            var damageable = other.collider.gameObject.GetComponentInChildren<IDamageable>();
            if (damageable == null) return;
            damageable.TakeDamage(damage, Vector2.zero, this);
            onDamage.Invoke();
            if (damageSfx)
                damageSfx.Play();
        }

        public bool LayerInMask(int layer)
        {
            return layers == (layers | (1 << layer));
        }

        public void ScaleDamage(float scalar)
        {
            damage *= scalar;
        }
    }
}
