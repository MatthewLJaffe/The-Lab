using EntityStatsScripts;
using UnityEngine;

namespace General
{
    public class DamageSource : MonoBehaviour
    {
        public float damage;
        [SerializeField] protected Collider2D sourceCollider;
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
            collision.GetComponentInChildren<IDamageable>()?.TakeDamage(damage, Vector2.zero);
        }

        protected virtual void OnCollisionEnter2D(Collision2D other)
        {
            //check if layer is in layer mask
            if ( !LayerInMask(other.gameObject.layer)) return;
            other.gameObject.GetComponentInChildren<IDamageable>()?.TakeDamage(damage, Vector2.zero);
        }

        public bool LayerInMask(int layer)
        {
            return layers == (layers | (1 << layer));
        }
    }
}
