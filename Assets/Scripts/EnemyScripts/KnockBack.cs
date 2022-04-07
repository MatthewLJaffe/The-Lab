using System;
using UnityEngine;

namespace EnemyScripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class KnockBack : MonoBehaviour
    {
        [SerializeField] private float multiplier;
        [SerializeField] private float maxKnockBack;
        [SerializeField] private Enemy enemy;
        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            enemy.enemyKilled += delegate { enabled = false; };
        }

        public void ApplyKnockBack(float amount)
        {
            ApplyKnockBack(amount, -_rb.velocity);
        }

        public void ApplyKnockBack(float amount, Vector2 dir)
        {
            if (!enabled) return;
            var force = Mathf.Clamp(amount * multiplier, 0, maxKnockBack);
            _rb.AddForce(dir.normalized * (force * _rb.mass), ForceMode2D.Impulse);
        }
    }
}