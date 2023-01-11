using General;
using UnityEngine;
using UnityEngine.Events;
using WeaponScripts;

namespace EnemyScripts
{
    /// <summary>
    /// used by enemies to instantiate bullets
    /// </summary>
    public class EnemyShoot : MonoBehaviour, IFire
    {
        public UnityEvent onShoot;
        public Transform[] shootPoints;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float timeBetweenShots;
        [SerializeField] private float damage;
        [SerializeField] private float bulletSpeed;
        private float _shootCooldown;

        public bool CanShoot
        {
            get => _canShoot;
            set
            {
                //reset shoot cooldown when able to shoot
                if (value && !_canShoot)
                    _shootCooldown = 0;
                _canShoot = value;
            }
        }
        private bool _canShoot;
        private Enemy _enemy;
        private GameObjectPool _bulletPool;


        private void Awake()
        {
            _enemy = GetComponentInParent<Enemy>();
            _enemy.enemyKilled += delegate { enabled = false; };
            _bulletPool = new GameObjectPool(bulletPrefab);
        }

        private void FixedUpdate()
        {
            if (!_canShoot) return;
            if (_shootCooldown >= timeBetweenShots)
            {
                _shootCooldown = 0;
                Fire();
            }
            else {
                _shootCooldown += Time.fixedDeltaTime;
            }
        }

        public virtual void Fire()
        {
            onShoot.Invoke();
            foreach (var shootPoint in shootPoints)          
            {
                var bulletGo = _bulletPool.GetFromPool();
                bulletGo.transform.position = shootPoint.position;
                var bulletComponent = bulletGo.GetComponent<PooledBullet>();
                bulletComponent.firedBy = _enemy.gameObject;
                bulletComponent.direction =  shootPoint.localRotation * -transform.up;
                bulletComponent.speed = bulletSpeed;
                bulletComponent.damage = damage;
            }
        }

        public void ScaleDamage(float scalar)
        {
            damage *= scalar;
        }
    }
}