using General;
using UnityEngine;


namespace WeaponScripts
{
    public class PooledBullet : Bullet, IPooled
    {
        public GameObjectPool MyPool { get; set; }
        
        public void ReturnToMyPool()
        {
            if (MyPool == null)
            {
                Destroy(gameObject);
                return;
            }
            if (BulletReturnRoutine != null)
                StopCoroutine(BulletReturnRoutine);
            direction = Vector2.zero;
            if (Particle && Particle.isPlaying)
            {
                Particle.Clear();
                Particle.Stop();
                Particle.time = 0;
            }
            MyPool.ReturnToPool(gameObject);

        }

        //called by BulletDestroy Animation Event
        public override void RemoveBullet()
        {
            ReturnToMyPool();
        }
    }
}
