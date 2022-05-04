using System;
using System.Collections;
using EntityStatsScripts;
using UnityEngine;
using UnityEngine.Events;

namespace LabCreationScripts
{
    public class CrateDamaged : MonoBehaviour, IDamageable
    {
        [SerializeField] private Sprite damagedSprite;
        [SerializeField] private SpriteRenderer sr;
        public UnityEvent onTakeDamage;
        [SerializeField] private CircleCollider2D damageCollider;
        [SerializeField] private float explosionTime;
        [SerializeField] private float explosionHitBoxDelay;
        [HideInInspector] public bool destroyed;

        private void Awake()
        {
            damageCollider.enabled = false;
        }
        
        public void TakeDamage(float amount, Vector2 dir)
        {
            onTakeDamage.Invoke();
            sr.sprite = damagedSprite;
            destroyed = true;
            StartCoroutine(DamageHitbox());
        }

        private void CheckForOtherCrates()
        {
            var hits = Physics2D.CircleCastAll(transform.position, damageCollider.radius, Vector2.zero,
                0f, LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer)));
            foreach (var hit in hits)
            {
                var crate = hit.transform.GetComponent<CrateDamaged>();
                if (crate && !crate.destroyed)
                    crate.TakeDamage(0f, Vector2.zero);
            }
            
        }

        private IEnumerator DamageHitbox()
        {
            yield return new WaitForSeconds(explosionHitBoxDelay);
            CheckForOtherCrates();
            damageCollider.enabled = true;
            yield return new WaitForSeconds(explosionTime);
            damageCollider.enabled = false;
        }
    }
}