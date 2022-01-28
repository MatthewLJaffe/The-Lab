using System.Collections;
using EntityStatsScripts;
using General;
using UnityEngine;

namespace WeaponScripts
{
 public class NonPooledBullet : DamageSource
    {
        public float accuracy;
        public Vector2 direction;
        [Tooltip("Set this to nonzero to give the bullet a fixed speed leave at zero to give bullet speed based on accuracy")]
        public float speed;
        private BoxCollider2D _destructionCollider;
        private Animator _animator;
        [SerializeField] protected float liveTime;
        protected Rigidbody2D rb;

        protected override void Awake()
        {
            base.Awake();
            rb = GetComponent<Rigidbody2D>();
            _destructionCollider = GetComponent<BoxCollider2D>();
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
            if (_destructionCollider.IsTouching(other))
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

        private IEnumerator DestroyBullet()
        {
            yield return new WaitForSeconds(liveTime);
            Destroy(gameObject);
        }

        private void DestroyBulletNow()
        {
            Destroy(gameObject);
        }

        protected IEnumerator InitializeBullet()
        {
            yield return new WaitUntil(() => direction != Vector2.zero);
            StartCoroutine(DestroyBullet());
            transform.rotation = GetRotation();
            rb.velocity = transform.rotation * Vector2.up * speed;
        }
    }
}