using System;
using System.CodeDom;
using General;
using UnityEngine;
using WeaponScripts;

namespace EnemyScripts
{
    public class EnemyShoot : MonoBehaviour, IFire
    {
        [SerializeField] private Transform[] shootPoints;
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
            _bulletPool = new GameObjectPool(bulletPrefab);
        }

        private void FixedUpdate()
        {
            if (_enemy.target)
            {
                var trans = transform;
                trans.up = trans.position - _enemy.target.position;
            }
            
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

        public void Fire()
        {
            foreach (var shootPoint in shootPoints)          
            {
                var bulletGo = _bulletPool.GetFromPool();
                bulletGo.transform.position = shootPoint.position;
                var bulletComponent = bulletGo.GetComponent<PooledBullet>();
                bulletComponent.direction =  shootPoint.localRotation * -transform.up;
                bulletComponent.speed = bulletSpeed;
                bulletComponent.damage = damage;
            }
        }
    }
}