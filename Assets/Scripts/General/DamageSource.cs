using System;
using EntityStatsScripts;
using UnityEngine;

namespace General
{
    public class DamageSource : MonoBehaviour
    {
        public float damage;
        [SerializeField] protected Collider2D sourceCollider;

        [SerializeField] private SoundEffect damageSfx;
        public LayerMask layers;

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
            damageable.TakeDamage(damage, Vector2.zero);
            if (damageSfx)
                damageSfx.Play();
        }

        protected virtual void OnCollisionEnter2D(Collision2D other)
        {
            //check if layer is in layer mask
            if ( !LayerInMask(other.gameObject.layer)) return;
            var damageable = other.gameObject.GetComponentInChildren<IDamageable>();
            if (damageable == null) return;
            damageable.TakeDamage(damage, Vector2.zero);
            if (damageSfx)
                damageSfx.Play();
        }

        public bool LayerInMask(int layer)
        {
            return layers == (layers | (1 << layer));
        }
    }
}
