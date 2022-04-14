using System;
using UnityEngine;

namespace WeaponScripts
{
    public class PlayerBullet : PooledBullet
    {
        public static Action<PlayerBullet> bulletDamage = delegate { };
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject == gameObject) return;
            if (sourceCollider.isTrigger && direction != Vector2.zero &&
                layers == (layers | (1 << other.gameObject.layer)))  {
                bulletDamage.Invoke(this);
            }
            base.OnTriggerEnter2D(other);
        }
    }
}