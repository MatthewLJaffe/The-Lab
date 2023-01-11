using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using WeaponScripts;
using Random = UnityEngine.Random;

namespace EnemyScripts
{
    /// <summary>
    /// attack for scientist that holds on to and throws explosive flask at player
    /// </summary>
    public class SpikeFlask : MonoBehaviour, IFire
    {
        [SerializeField] private GameObject flask;
        [SerializeField] private Transform shootPoint;
        [SerializeField] private float fireDelay;
        [SerializeField] private float throwMag;
        [SerializeField] private AnimationCurve throwCurve;
        [SerializeField] private float throwSpeed;
        [SerializeField] private float projectileSpeed;
        [SerializeField] private Vector2 cooldownRange;
        [SerializeField] private Enemy enemy;
        [SerializeField] private SpriteRenderer flaskDisplay;
        public UnityEvent onFire;
        private float _currCooldown;

        private void Awake()
        {
            _currCooldown = Random.Range(cooldownRange.x, cooldownRange.y);
            CanShoot = false;        
        }
        
        public bool CanShoot { get; set; }

        private void FixedUpdate()
        {
            if (!CanShoot) return;
            if (_currCooldown <= 0)
            {
                Fire();
                _currCooldown = Random.Range(cooldownRange.x, cooldownRange.y);
            }
            else
                _currCooldown -= Time.fixedDeltaTime;

        }

        public void Fire()
        {
            var dir = (enemy.target.position - shootPoint.position).normalized * throwMag;
            StartCoroutine(ThrowFlask(dir));
        }

        private IEnumerator ThrowFlask( Vector2 dir)
        {
            flaskDisplay.enabled = true;
            yield return new WaitForSeconds(fireDelay);
            flaskDisplay.enabled = false;
            var flaskInstance = Instantiate(flask,  shootPoint.position, Quaternion.identity, null).GetComponent<ScientistFlask>();
            flaskInstance.projectileSpeed = projectileSpeed;
            flaskInstance.flying = false;
            flaskInstance.enemy = enemy;
            flaskInstance.transform.parent = null;
            var throwTime = dir.magnitude / throwSpeed;
            var normalizedDir = dir.normalized;
            onFire.Invoke();
            for (var t = 0f; t < throwTime; t += Time.fixedDeltaTime)
            {
                flaskInstance.rb.velocity = normalizedDir * (throwCurve.Evaluate(t) * throwSpeed);
                yield return new WaitForFixedUpdate();
            }
            flaskInstance.Explode();
        }
    }
}