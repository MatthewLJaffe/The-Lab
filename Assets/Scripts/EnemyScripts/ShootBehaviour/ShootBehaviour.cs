using System;
using General;
using UnityEngine;
using UnityEngine.Events;

namespace EnemyScripts
{
        /// <summary>
        /// gives shoot controller a way of interfacing with different projectile attack patterns
        /// </summary>
        public abstract class ShootBehaviour : MonoBehaviour
        {
                [SerializeField] protected GameObject bulletPrefab;
                public UnityEvent onShoot;
                public float coolDown;
                [SerializeField] protected float damage;
                public float bulletSpeed;
                protected GameObjectPool BulletPool;
                
                protected virtual void Awake()
                { 
                        BulletPool = new GameObjectPool(bulletPrefab);
                }

                public abstract void Shoot(Transform shootPoint, Enemy enemy);
        }
}