using System;
using System.Linq;
using UnityEngine;

namespace General
{
    public class UnequalDamageSource : DamageSource
    {
        [SerializeField] private LayerDamage[] layerDamages;
        [Serializable]
        private struct LayerDamage
        {
            public string layer;
            public float damage;
        }
        
        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if ( !LayerInMask(collision.gameObject.layer)) return;
            var layerName = LayerMask.LayerToName(collision.gameObject.layer);
            damage = layerDamages.First(ld => ld.layer == layerName).damage;
            base.OnTriggerEnter2D(collision);
        }

        protected override void OnCollisionEnter2D(Collision2D other)
        {
            if ( !LayerInMask(other.gameObject.layer)) return;
            var layerName = LayerMask.LayerToName(other.gameObject.layer);
            damage = layerDamages.First(ld => ld.layer == layerName).damage;
            base.OnCollisionEnter2D(other);
        }

    }
}