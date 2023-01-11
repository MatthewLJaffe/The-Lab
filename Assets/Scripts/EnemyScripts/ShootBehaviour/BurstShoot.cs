using System.Collections;
using UnityEngine;
using WeaponScripts;

namespace EnemyScripts
{
    public class BurstShoot : ShootBehaviour
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
                var bulletGo = Instantiate(bulletPrefab);
                bulletGo.transform.localRotation = 
                    Quaternion.Euler(0,0, -burstAngle/2 + c * (burstAngle / (burstNum - 1)));
                bulletGo.transform.position = shootPoint.position;
                var bulletComponent = bulletGo.GetComponent<Bullet>();
                bulletComponent.firedBy = enemy.gameObject;
                bulletComponent.direction =  bulletGo.transform.localRotation * -transform.up;
                bulletComponent.speed = bulletSpeed;
                bulletComponent.damage = damage;
                yield return new WaitForSeconds(burstRate);
            }
        }
    }
}