using System.Collections;
using UnityEngine;
using WeaponScripts;

namespace EnemyScripts
{
    public class SprayShoot : ShootBehaviour
    {
        [SerializeField] private int burstNum;
        [SerializeField] private float burstRate;
        [SerializeField] private float burstAngle;
        public override void Shoot(Transform shootPoint, Enemy enemy)
        {
            StartCoroutine(FireBurst(shootPoint, enemy));
        }

        private IEnumerator FireBurst(Transform shootPoint, Enemy enemy)
        {
            for (int c = 0; c < burstNum; c++)
            {
                onShoot.Invoke();
                var bulletGo = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
                var bulletComponent = bulletGo.GetComponent<Bullet>();
                bulletComponent.firedBy = enemy.gameObject;
                bulletComponent.direction =  enemy.target.position - shootPoint.position;
                bulletComponent.speed = bulletSpeed;
                bulletComponent.damage = damage;
                yield return new WaitForSeconds(burstRate);
            }
        }
    }
}