using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using WeaponScripts;

namespace EnemyScripts
{
    public class PredictionShoot : MonoBehaviour, IFire
    {
        [SerializeField] private Enemy enemy;
        [SerializeField] private GameObject projectile;
        [SerializeField] private float shootDelay;
        [SerializeField] private Transform shootPoint;
        [SerializeField] private int predictionFrames= 5;
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
            yield return new WaitForSeconds(Time.fixedDeltaTime * predictionFrames);
            var acc = (velocity - velInitial) / predictionFrames;
            var projectileSpeed = projectile.GetComponent<NonPooledBullet>().speed;
            var travelTime = Vector2.Distance(targetRb.transform.position, transform.position) / projectileSpeed;
            var targetDest = (Vector2) enemy.target.transform.position + velocity * (shootDelay + travelTime) +
                             .5f * Mathf.Pow((shootDelay + travelTime), 2) * acc;
            ShootDir = targetDest - (Vector2)transform.position;

        }

        public void Fire()
        {
            var projInstance = Instantiate(projectile, shootPoint.transform.position, quaternion.identity);
            projInstance.transform.up = ShootDir;
            projInstance.GetComponent<NonPooledBullet>().direction = ShootDir;
        }

    }
}