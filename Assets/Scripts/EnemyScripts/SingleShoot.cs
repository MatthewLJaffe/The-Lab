using System;
using UnityEngine;
using WeaponScripts;

namespace EnemyScripts
{
    public class SingleShoot : ShootBehaviour
    {

        public override void Shoot(Transform shootPoint, Enemy enemy)
        {
            var ps = shootPoint.GetComponent<ParticleSystem>();
            if (ps)
                ps.Play();
            onShoot.Invoke();
            var bulletGo = BulletPool.GetFromPool();
            bulletGo.transform.position = shootPoint.position;
            var bulletComponent = bulletGo.GetComponent<PooledBullet>();
            bulletComponent.firedBy = enemy.gameObject;
            bulletComponent.direction =  shootPoint.localRotation * -transform.up;
            bulletComponent.speed = bulletSpeed;
            bulletComponent.damage = damage;
        }
    }
}