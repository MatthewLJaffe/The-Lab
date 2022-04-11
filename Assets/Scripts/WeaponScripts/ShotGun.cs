using System;
using System.Collections;
using General;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WeaponScripts
{
    public class ShotGun : Gun
    {
        [SerializeField] private float maxSpreadAngle;
        [SerializeField] private int bullets;


        protected override void ShootProjectile()
        {
            if (!mainCamera.gameObject.activeSelf) return;
            shootSound.Play(fireSource);
            var spreadAngle = maxSpreadAngle * Mathf.Max(0, 1f - (gunStats.accuracy + additionalAccuracy) / 100f);
            for (var c = 0; c < bullets; c++)
            {
                var angle = -spreadAngle / 2 + c * spreadAngle / (bullets - 1);
                
                var bulletInstance = _bulletPool.GetFromPool();
                bulletInstance.transform.position = shootPoint.position;
                var bulletComponent = bulletInstance.GetComponent<PooledBullet>();
                var mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                bulletComponent.damage = Mathf.Max(1f, gunStats.damage * atkMult);
                bulletComponent.crit = gunStats.critChance + playerCritChance > Random.Range(0f, 100f);

                //altering direction and leaving accuracy at 100
                bulletComponent.accuracy = 100f;
                bulletComponent.speed = gunStats.accuracy / 8f + 4;
                if (Vector2.Distance(mousePos, playerTrans.position) > 1)
                    bulletComponent.direction = (mousePos - shootPoint.position).normalized;
                else
                    bulletComponent.direction = (mousePos - playerTrans.position).normalized;
                bulletComponent.direction = Quaternion.Euler(0, 0, angle) * bulletComponent.direction;
            }
        }
    }
}