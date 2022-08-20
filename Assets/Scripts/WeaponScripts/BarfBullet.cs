using System.Collections;
using UnityEngine;

namespace WeaponScripts
{
    public class BarfBullet : Bullet
    {
        [SerializeField] private int numParticles;
        [SerializeField] private Transform destroyStart;
        [SerializeField] private Transform destroyEnd;
        [SerializeField] private GameObject destroyParticle;
        protected override IEnumerator PlayParticle()
        {
            for (var c = 1; c <= numParticles; c++)
            {
                var raycastPos = Vector3.Lerp(destroyStart.position, destroyEnd.position, (c / (float) numParticles));
                var hit = Physics2D.Raycast(raycastPos, transform.up, 3f,
                    Physics2D.GetLayerCollisionMask(LayerMask.NameToLayer("Enemy Bullet")));
                if (hit) {
                    Instantiate(destroyParticle, hit.point, Quaternion.identity);
                }
            }
            yield break;
        }
    }
}