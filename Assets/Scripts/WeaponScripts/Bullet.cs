using System;
using System.Collections;
using EntityStatsScripts;
using General;
using PlayerScripts;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace WeaponScripts
{
    public class Bullet : DamageSource, IPooled
    {
        public float accuracy;
        public Vector2 direction;
        [Tooltip("Set this to nonzero to give the bullet a fixed speed leave at zero to give bullet speed based on accuracy")]
        public float speed;
        private CapsuleCollider2D _capsuleCollider;
        public GameObjectPool MyPool { get; set; }
        protected Coroutine BulletReturnRoutine;
        private Animator _animator;
        [SerializeField] protected float liveTime;
        protected Rigidbody2D rb;

        protected override void Awake()
        {
            base.Awake();
            rb = GetComponent<Rigidbody2D>();
            _capsuleCollider = GetComponent<CapsuleCollider2D>();
            _animator = GetComponent<Animator>();
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
            if (sourceCollider.isTrigger && direction != Vector2.zero &&layers == (layers | (1 << other.gameObject.layer)))
                damageable?.TakeDamage(damage, rb.velocity, this);
            if (_capsuleCollider.IsTouching(other) || damageable != null)
            {
                rb.velocity = Vector2.zero;
                _animator.SetTrigger("Destroy");
                direction = Vector2.zero;
            }
        }

        protected Quaternion GetRotation()
        {
            var angle = Vector2.SignedAngle(Vector2.up, direction);
            float angleOff = 45f * Random.Range(-(1f - accuracy / 100f), 1f - accuracy / 100f);
            Quaternion rot = Quaternion.Euler(0, 0, angle + angleOff );
            return rot;
        }
        //called by BulletDestroy Animation Event
        public void ReturnToMyPool()
        {
            if (BulletReturnRoutine != null)
                StopCoroutine(BulletReturnRoutine);
            direction = Vector2.zero;
            MyPool.ReturnToPool(gameObject);
        }

        protected IEnumerator ReturnBullet()
        {
            yield return new WaitForSeconds(liveTime);
            rb.velocity = Vector2.zero;
            _animator.SetTrigger("Destroy");
        }

        protected IEnumerator InitializeBullet()
        {
            yield return new WaitUntil(() => direction != Vector2.zero);
            BulletReturnRoutine = StartCoroutine(ReturnBullet());
            transform.rotation = GetRotation();
            rb.velocity = transform.rotation * Vector2.up * speed;
        }
    }
}
