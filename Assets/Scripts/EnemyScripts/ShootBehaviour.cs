using System;
using General;
using UnityEngine;
using UnityEngine.Events;

namespace EnemyScripts
{
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