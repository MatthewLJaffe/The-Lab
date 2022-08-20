using System.Collections;
using EnemyScripts;
using UnityEngine;
using UnityEngine.Events;
using WeaponScripts;

namespace LabCreationScripts
{
    public class WallShooter : MonoBehaviour, IFire
    {
        [SerializeField] private GameObject trapBullet;
        [SerializeField] private Vector2 dir;
        [SerializeField] private Transform[] shootPoints;
        [SerializeField] private float bulletDamage;
        [SerializeField] private float bulletSpeed;
        [SerializeField] private float shootCooldown;
        public UnityEvent onFire;
        public bool CanShoot { get; set; } = true;


        private IEnumerator CooldownRoutine()
        {
            CanShoot = false;
            yield return new WaitForSeconds(shootCooldown);
            CanShoot = true;
        }
        
        public void Fire()
        {
            if (!CanShoot) return;
            StartCoroutine(CooldownRoutine());
            onFire.Invoke();
            foreach (var point in shootPoints)
            {
                var bulletComponent = Instantiate(trapBullet, point.position, Quaternion.identity)
                    .GetComponent<Bullet>();
                bulletComponent.direction = dir;
                bulletComponent.speed = bulletSpeed;
                bulletComponent.damage = bulletDamage;
            }
        }
    }
}