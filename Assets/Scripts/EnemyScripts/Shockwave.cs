using System;
using EntityStatsScripts;
using General;
using UnityEngine;

namespace EnemyScripts
{
    public class Shockwave : MonoBehaviour
    {
        [SerializeField] private SoundEffect stompSound;
        [SerializeField] private CircleCollider2D innerCollider;
        [SerializeField] private float damage;

        private void Start()
        {
            stompSound.Play();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.layer != LayerMask.NameToLayer("Player")) return;
            if (!innerCollider.IsTouching(col))
                col.GetComponentInChildren<IDamageable>().TakeDamage(damage, Vector2.zero);
        }

        public void DestroyShockwave()
        {
            Destroy(gameObject);
        }
    }
}