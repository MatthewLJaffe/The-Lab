using System;
using System.Collections.Generic;
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
        private Rigidbody2D _rb;
        private List<GameObject> _enteredObjects;

        protected override void Awake()
        {
            base.Awake();
            _rb = GetComponentInParent<Rigidbody2D>();
            _enteredObjects = new List<GameObject>();
        }
        
        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (LayerInMask(collision.gameObject.layer) && !_enteredObjects.Contains(collision.gameObject))
                _enteredObjects.Add(collision.gameObject);
        }

        protected override void OnCollisionEnter2D(Collision2D other) { }

        protected void OnTriggerStay2D(Collider2D other)
        {
            if (!enabled) return;
            if (!_enteredObjects.Remove(other.gameObject)) return;
            damage = ComputeDamage();
            base.OnTriggerEnter2D(other);
        }
        
        private float ComputeDamage()
        {
            return minDamage + 
                   Mathf.Round(Mathf.Clamp(_rb.velocity.magnitude / maxSpeed * (maxDamage - minDamage), 0, (maxDamage - minDamage)));
        }
    }
}