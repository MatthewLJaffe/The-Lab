using System;
using System.Collections;
using EntityStatsScripts;
using General;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace WeaponScripts
{
 public class Bullet : DamageSource
 {
     public static Action<Bullet> BulletDamage;
        public bool crit;
        public float accuracy;
        public Vector2 direction;
        [Tooltip("Set this to nonzero to give the bullet a fixed speed leave at zero to give bullet speed based on accuracy")]
        public float speed;
        [SerializeField] protected float liveTime;
        [SerializeField] protected float maxAngle;
        [SerializeField] protected bool destroyOnDamage;
        [SerializeField] protected Collider2D destructionCollider;
        protected Animator Animator;
        protected Coroutine BulletReturnRoutine;
        protected Rigidbody2D Rb;
        protected ParticleSystem Particle;

        protected override void Awake()
        {
            base.Awake();
            Rb = GetComponent<Rigidbody2D>();
            Animator = GetComponent<Animator>();
            Particle = GetComponent<ParticleSystem>();
        }

        protected void OnEnable()
        {
            StartCoroutine(InitializeBullet());
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject == gameObject) return;
            var damageable = other.GetComponentInChildren<IDamageable>();
            //using direction as a way to tell if the bullet is live
            //check to see if other layer is damageable
            if (sourceCollider.isTrigger && direction != Vector2.zero &&
                layers == (layers | (1 << other.gameObject.layer)))
            {
                BulletDamage.Invoke(this);
                damageable?.TakeDamage(damage, Rb.velocity);
            }
            if (destructionCollider.IsTouching(other) || destroyOnDamage && damageable != null)
            {
                StartBulletDestruction();
            }
        }

        private void StartBulletDestruction()
        {
            Rb.velocity = Vector2.zero;
            if (Particle && !Particle.isPlaying) {
                Particle.Play();
            }
            Animator.SetTrigger("Destroy");
            direction = Vector2.zero;
        }

        protected Quaternion GetRotation()
        {
            var angle = Vector2.SignedAngle(Vector2.up, direction);
            float angleOff = maxAngle * Random.Range(-(1f - accuracy / 100f), 1f - accuracy / 100f);
            Quaternion rot = Quaternion.Euler(0, 0, angle + angleOff );
            return rot;
        }

        protected virtual IEnumerator BulletLifetime()
        {
            yield return new WaitForSeconds(liveTime);
            StartBulletDestruction();
        }

        //Called by animation event
        public virtual void RemoveBullet()
        {
            if (BulletReturnRoutine != null)
                StopCoroutine(BulletReturnRoutine);
            Destroy(gameObject);
        }

        protected IEnumerator InitializeBullet()
        {
            yield return new WaitUntil(() => direction != Vector2.zero);
            if (crit)
                damage *= 5;
            BulletReturnRoutine = StartCoroutine(BulletLifetime());
            transform.rotation = GetRotation();
            Rb.velocity = transform.rotation * Vector2.up * speed;
        }
    }
}