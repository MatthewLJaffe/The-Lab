using System.Collections;
using UnityEngine;
using WeaponScripts;

namespace EnemyScripts
{
    public class ClusterShoot : ShootBehaviour
    {
        [SerializeField] private int burstNum;
        [SerializeField] private float burstAngle;
        public float rotOffset;
        public override void Shoot(Transform shootPoint, Enemy enemy)
        {
            FireCluster(shootPoint, enemy);
        }

        private void FireCluster(Transform shootPoint, Enemy enemy)
        {
            for (int c = 0; c < burstNum; c++)
            {
                onShoot.Invoke();
                var bulletGo = Instantiate(bulletPrefab);
                bulletGo.transform.rotation = 
                    Quaternion.Euler(0,0, burstAngle * (c / (float)burstNum) + rotOffset);
                bulletGo.transform.position = shootPoint.position;
                var bulletComponent = bulletGo.GetComponent<Bullet>();
                bulletComponent.firedBy = enemy.gameObject;
                bulletComponent.direction =  bulletGo.transform.localRotation * -transform.up;
                bulletComponent.speed = bulletSpeed;
                bulletComponent.damage = damage;
            }
        }
    }
}