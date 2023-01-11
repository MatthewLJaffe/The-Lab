using System.Collections;
using UnityEngine;
using WeaponScripts;

namespace EnemyScripts
{
    public class BeegSpikeFlask : ShootBehaviour
    {
        [SerializeField] private SpriteRenderer flaskDisplay;
        [SerializeField] private float fireDelay;
        [SerializeField] private float throwMag;
        [SerializeField] private AnimationCurve throwCurve;
        [SerializeField] private float throwSpeed;
        
        public override void Shoot(Transform shootPoint, Enemy enemy)
        {
            var dir = (enemy.target.position - shootPoint.position).normalized * throwMag;
            StartCoroutine(ThrowFlask(dir, shootPoint, enemy));
        }
        
        private IEnumerator ThrowFlask(Vector2 dir, Transform shootPoint, Enemy enemy)
        {
            flaskDisplay.enabled = true;
            yield return new WaitForSeconds(fireDelay);
            flaskDisplay.enabled = false;
            var flaskInstance = Instantiate(bulletPrefab,  shootPoint.position, Quaternion.identity, null).GetComponent<ScientistFlask>();
            flaskInstance.projectileSpeed = bulletSpeed;
            flaskInstance.flying = false;
            flaskInstance.enemy = enemy;
            flaskInstance.transform.parent = null;
            var throwTime = dir.magnitude / throwSpeed;
            var normalizedDir = dir.normalized;
            onShoot.Invoke();
            for (var t = 0f; t < throwTime; t += Time.fixedDeltaTime)
            {
                flaskInstance.rb.velocity = normalizedDir * (throwCurve.Evaluate(t) * throwSpeed);
                yield return new WaitForFixedUpdate();
            }
            flaskInstance.Explode();
        }
    }
}