using UnityEngine;
using WeaponScripts;

namespace EnemyScripts
{
    public class BeegSprayBulletFlask : ShootBehaviour
    {
        [SerializeField] private float predictFactor;
        [SerializeField] private float flaskSpeed;
        [SerializeField] private float flaskSpacing;
        private Rigidbody2D _targetRb;

        public override void Shoot(Transform shootPoint, Enemy enemy)
        {
            Vector2 dir = shootPoint.position - enemy.target.position;
            if (_targetRb == null)
                _targetRb = enemy.target.GetComponent<Rigidbody2D>();
            var throwTarget = 
                _targetRb.velocity * (dir.magnitude * predictFactor / flaskSpeed) + (Vector2)enemy.target.position;
            var tangent = Vector3.Cross(dir, Vector3.forward).normalized;
            var rightTarget = throwTarget + (Vector2)tangent * (flaskSpacing * .5f);
            var leftTarget = throwTarget - (Vector2)tangent * (flaskSpacing * .5f);

            onShoot.Invoke();
            
            var leftFlask = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity)
                .GetComponentInChildren<ScientistFlask>();
            leftFlask.projectileSpeed = bulletSpeed;
            leftFlask.flying = true;
            leftFlask.speed = flaskSpeed;
            leftFlask.enemy = enemy;
            leftFlask.destination = leftTarget;
            
            var rightFlask = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity)
                .GetComponentInChildren<ScientistFlask>();
            rightFlask.projectileSpeed = bulletSpeed;
            rightFlask.flying = true;
            rightFlask.speed = flaskSpeed;
            rightFlask.enemy = enemy;
            rightFlask.destination = rightTarget;
            
        }
    }
}