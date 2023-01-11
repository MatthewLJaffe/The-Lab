using System.Collections;
using UnityEngine;
using WeaponScripts;

namespace EnemyScripts
{
    public class BeegFlaskHurl : ShootBehaviour
    {
        private Vector2 _throwTarget;
        [SerializeField] private int lobs;
        [SerializeField] private float rotOffset;
        [SerializeField] private float timeBetweenLobs;
        [SerializeField] private float predictFactor;
        [SerializeField] private float flaskSpeed;
        private Rigidbody2D _targetRb;

        //begone bullet pool
        protected override void Awake() { }

        public override void Shoot(Transform shootPoint, Enemy enemy)
        {
            ComputeThrowTarget(shootPoint, enemy);
            StartCoroutine(LobFlasks(shootPoint, enemy));
        }


        private IEnumerator LobFlasks(Transform shootPoint, Enemy enemy)
        {
            for (var c = 0; c < lobs; c++)
            {
                onShoot.Invoke();
                var flaskComponent = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity)
                    .GetComponentInChildren<ScientistFlask>();
                flaskComponent.projectileSpeed = bulletSpeed;
                flaskComponent.rotOffset = c * rotOffset;
                flaskComponent.flying = true;
                flaskComponent.speed = flaskSpeed;
                flaskComponent.enemy = enemy;
                flaskComponent.destination = _throwTarget;
                yield return new WaitForSeconds(timeBetweenLobs);
            }
        }

        private void ComputeThrowTarget(Transform shootPoint, Enemy enemy)
        {
            Vector2 dir = shootPoint.position - enemy.target.position;
            if (_targetRb == null)
                _targetRb = enemy.target.GetComponent<Rigidbody2D>();
            _throwTarget = 
                _targetRb.velocity * (dir.magnitude * predictFactor / flaskSpeed) + (Vector2)enemy.target.position;
        }
    }
}