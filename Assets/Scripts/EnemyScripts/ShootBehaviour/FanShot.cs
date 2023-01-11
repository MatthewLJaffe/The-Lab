using System.Collections;
using UnityEngine;
using WeaponScripts;

namespace EnemyScripts
{
    public class FanShot : ShootBehaviour
    {
        [SerializeField] private int numShots;
        [SerializeField] private float fanRate;
        [SerializeField] private float timeStep;
        
        public override void Shoot(Transform shootPoint, Enemy enemy)
        {
            StartCoroutine(FireFan(shootPoint, enemy));
        }

        private IEnumerator FireFan(Transform shootPoint, Enemy enemy)
        {
            var target = enemy.target;
            var playerRb = target.GetComponent<Rigidbody2D>();
            for (var c = 0; c < numShots; c++)
            {
                onShoot.Invoke();
                var shootDir = (Vector2)(target.position - shootPoint.position);
                var travelTime = 
                    Vector2.Distance(shootPoint.position, target.position) / bulletSpeed + c*timeStep;
                shootDir += travelTime * playerRb.velocity;
                var bulletGo = BulletPool.GetFromPool();
                bulletGo.transform.position = shootPoint.position;
                var bulletComponent = bulletGo.GetComponent<PooledBullet>();
                bulletComponent.firedBy = enemy.gameObject;
                bulletComponent.direction =  shootDir;
                bulletComponent.speed = bulletSpeed;
                bulletComponent.damage = damage;
                yield return new WaitForSeconds(fanRate);
            }
        }
    }
}