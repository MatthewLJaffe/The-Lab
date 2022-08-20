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
     [SerializeField] protected Collider2D destructionCollider;
     public bool crit;
     public float accuracy;
     public Vector2 direction;
     [Tooltip("Set this to nonzero to give the bullet a fixed speed leave at zero to give bullet speed based on accuracy")]
     public float speed;
     [SerializeField] protected float liveTime;
     [SerializeField] protected float maxAngle;
     [SerializeField] protected bool destroyOnDamage;
     [SerializeField] private float colorPickOffset;
     [SerializeField] private float normalAlignmentPriority = .75f;
     protected Animator Animator;
     protected Coroutine BulletReturnRoutine;
     protected Rigidbody2D Rb;
     protected ParticleSystem Particle;
     protected ParticleSystem.MainModule settings;
     private Camera _mainCamera;
     public GameObject firedBy;
        

        protected override void Awake()
        {
            base.Awake();
            Rb = GetComponent<Rigidbody2D>();
            Animator = GetComponent<Animator>();
            Particle = GetComponent<ParticleSystem>();
            if (Particle)
                settings = GetComponent<ParticleSystem>().main;
            _mainCamera = Camera.main;
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
                damageable?.TakeDamage(damage, Rb.velocity, this, crit);
                if (damageable != null)
                    onDamage.Invoke();
            }
            if (destructionCollider.IsTouching(other) && !other.isTrigger || destroyOnDamage && damageable != null)
            {
                BulletExplode();
            }
        }

        public void BulletExplode()
        {
            if (Particle) {
                StartCoroutine(PlayParticle());
            }
            if (damageSfx)
                damageSfx.Play();

            var hit = Physics2D.Raycast((Vector2) transform.position - Rb.velocity.normalized * .1f, Rb.velocity, 10f,
                Physics2D.GetLayerCollisionMask(gameObject.layer));
            transform.up = Vector3.Lerp(transform.up, -hit.normal, normalAlignmentPriority); 
            StartBulletDestruction();
            foreach (var coll in gameObject.GetComponentsInChildren<Collider2D>()) {
                coll.enabled = false;
            }
        }

        protected virtual IEnumerator PlayParticle()
        {
            yield return new WaitForEndOfFrame();
            var viewRect = _mainCamera.pixelRect;
            Texture2D tex = new Texture2D( (int)viewRect.width, (int)viewRect.height);
            tex.ReadPixels( viewRect, 0, 0, false );
            tex.Apply( false );
            var pixelCoord = _mainCamera.WorldToScreenPoint(transform.position + transform.up * colorPickOffset);
            settings.startColor = tex.GetPixel((int)pixelCoord.x, (int)pixelCoord.y);
            if (!Particle.isPlaying) {
                Particle.Play();
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
            Rb.velocity = Vector2.zero;
            Animator.SetTrigger("Destroy");
            direction = Vector2.zero;
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
            foreach (var coll in gameObject.GetComponentsInChildren<Collider2D>()) {
                coll.enabled = true;
            }
            yield return new WaitUntil(() => direction != Vector2.zero);
            BulletReturnRoutine = StartCoroutine(BulletLifetime());
            transform.rotation = GetRotation();
            Rb.velocity = transform.rotation * Vector2.up * speed;
        }
    }
}