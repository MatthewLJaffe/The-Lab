using System;
using System.Collections;
using EnemyScripts;
using General;
using UnityEngine;
using UnityEngine.Events;

namespace WeaponScripts
{
    public class ScientistFlask : DamageSource
    {
        public float speed;
        public float projectileSpeed;
        public Rigidbody2D rb;
        [SerializeField] private float rotFactor;
        [HideInInspector] public Vector2 destination;
        [SerializeField] private ShootBehaviour burst;
        [SerializeField] private Shadow shadow;
        public float rotOffset;
        public Enemy enemy;
        public UnityEvent atDestination;
        private float _distance;
        private Vector2 _startPos;
        public bool flying = true;

        private void Start()
        {
            burst.bulletSpeed = projectileSpeed;
            if (!flying) return;
            _startPos = transform.position;
            var dir = (destination - _startPos);
            _distance = Vector2.Distance(_startPos, destination);
            rb.velocity = dir.normalized * speed;
            StartCoroutine(Destruction( _distance / rb.velocity.magnitude));
        }

        private void Update()
        {
            if (!flying) return;
            transform.Rotate(Vector3.forward, rotFactor * Time.deltaTime);
            shadow.ShadowProgress(1 - Vector2.Distance(transform.position, destination) / _distance);
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (flying) {
                Explode(false);
            }
            base.OnTriggerEnter2D(collision);
        }

        private IEnumerator Destruction(float t)
        {
            yield return new WaitForSeconds(t);
            Explode(false);
        }

        public void Explode(bool alwaysDetonate = true)
        {
            if(!flying && !alwaysDetonate) return;
            flying = false;
            rb.velocity = Vector2.zero;
            atDestination.Invoke();
            transform.up = -(enemy.target.position - transform.position);
            if (burst.GetType() == typeof(ClusterShoot)) {
                ((ClusterShoot)burst).rotOffset = rotOffset;
            }
            if (enemy)
                burst.Shoot(transform, enemy);
        }

        public void DestroyFlask()
        {
            if (transform.parent)
                Destroy(transform.parent.gameObject);
            else
            {
                Destroy(gameObject);
            }
        }
    }
}