using System;
using System.Linq;
using EntityStatsScripts;
using LabCreationScripts.Spawners;
using UnityEngine;

namespace General
{
    public class StretcherDamageSource : DamageSource
    {
        [SerializeField] protected float maxDamage = 30f;
        [SerializeField] protected float minDamage = 5f;
        [SerializeField] protected float maxSpeed;
        [SerializeField] private Collider2D forwardCollider;
        [SerializeField] private Collider2D backwardCollider;
        [SerializeField] private StretcherSpawner.StretcherDirection dir;
        private bool _damagedOnCollision;
        
        protected Rigidbody2D rb;

        protected override void Awake()
        {
            base.Awake();
            rb = GetComponentInParent<Rigidbody2D>();
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
        }

        protected override void OnCollisionEnter2D(Collision2D other)
        {
            if (dir == StretcherSpawner.StretcherDirection.Vertical)
            {
                if (rb.velocity.y > 0 && !other.collider.IsTouching(forwardCollider)) return;
                if (rb.velocity.y < 0 && !other.collider.IsTouching(backwardCollider)) return;
            }
            if (dir == StretcherSpawner.StretcherDirection.Horizontal)
            {
                if (rb.velocity.x > 0 && !other.collider.IsTouching(forwardCollider)) return;
                if (rb.velocity.x < 0 && !other.collider.IsTouching(backwardCollider)) return;
            }
            if (other.gameObject.CompareTag("Player")) return;
            damage = ComputeDamage();
            _damagedOnCollision = true;
            base.OnCollisionEnter2D(other);
        }

        protected void OnCollisionStay2D(Collision2D other)
        {
            if (_damagedOnCollision) return;
            if (dir == StretcherSpawner.StretcherDirection.Vertical)
            {
                if (rb.velocity.y > 0 && !other.collider.IsTouching(forwardCollider)) return;
                if (rb.velocity.y < 0 && !other.collider.IsTouching(backwardCollider)) return;
            }
            if (dir == StretcherSpawner.StretcherDirection.Horizontal)
            {
                if (rb.velocity.x > 0 && !other.collider.IsTouching(forwardCollider)) return;
                if (rb.velocity.x < 0 && !other.collider.IsTouching(backwardCollider)) return;
            }
            if (other.gameObject.CompareTag("Player")) return;
            damage = ComputeDamage();
            _damagedOnCollision = true;
            base.OnCollisionEnter2D(other);
        }

        protected void OnCollisionExit2D(Collision2D other)
        {
            _damagedOnCollision = false;
        }

        private float ComputeDamage()
        {
            var d = Mathf.Round(Mathf.Clamp(rb.velocity.magnitude / maxSpeed * maxDamage, 0, maxDamage));
            if (d < minDamage)
                return 0;
            return d;
        }
    }
}