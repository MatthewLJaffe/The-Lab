using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using WeaponScripts;

namespace EnemyScripts
{
    public class PredictionShoot : MonoBehaviour, IFire
    {
        [SerializeField] private float damage;
        [SerializeField] private Enemy enemy;
        [SerializeField] private GameObject projectile;
        [SerializeField] private Transform shootPoint;
        [SerializeField] private float predictionTime;
        public Vector2 ShootDir { get; private set; }

        public bool CanShoot { get; set; }

        private void Awake()
        {
            if (enemy == null)
                enemy = GetComponent<Enemy>();
        }

        public void PredictShootDirection()
        {
            StartCoroutine(PredictDirection());
        }

        private IEnumerator PredictDirection()
        {
            var targetRb = enemy.target.GetComponent<Rigidbody2D>();
            var velocity = targetRb.velocity;
            var velInitial = new Vector2(velocity.x, velocity.y);
            yield return new WaitForSeconds(predictionTime);
            var acc = (velocity- velInitial) / predictionTime;
            var projectileSpeed = projectile.GetComponent<Bullet>().speed;
            var travelTime = Vector2.Distance(targetRb.transform.position, transform.position) / projectileSpeed;
            var targetDest = (Vector2) enemy.target.transform.position + targetRb.velocity * travelTime +
                             .5f * Mathf.Pow(travelTime, 2) * acc;
            ShootDir = targetDest - (Vector2)transform.position;
        }

        public void Fire()
        {
            var projInstance = Instantiate(projectile, shootPoint.transform.position, quaternion.identity);
            projInstance.transform.up = ShootDir;
            var bulletComponent = projInstance.GetComponent<Bullet>();
            bulletComponent.direction = ShootDir;
            bulletComponent.firedBy = enemy.gameObject;
            bulletComponent.damage = damage;
        }

        public void ScaleDamage(float scalar)
        {
            damage *= scalar;
        }

    }
}