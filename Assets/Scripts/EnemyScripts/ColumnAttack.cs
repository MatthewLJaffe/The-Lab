using System;
using System.Collections;
using System.Xml.Linq;
using General;
using UnityEngine;
using UnityEngine.Events;

namespace EnemyScripts
{
    public class ColumnAttack : MonoBehaviour
    {
        public Vector2 destination;
        public float speed;
        public float damage;
        [SerializeField] private DamageSource damageSource;
        [SerializeField] private ParticleSystem ps;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private BoxCollider2D collider;

        private void Start()
        {
            damageSource.damage = damage;
            var dir = destination - (Vector2)transform.position;
            transform.up = dir.normalized;
            rb.velocity = dir.normalized * speed;
            StartCoroutine(WaitForReachedDestination(dir.magnitude));

        }

        private IEnumerator WaitForReachedDestination(float distance)
        {
            var travelTime = distance / speed;
            collider.enabled = true;
            var fixedUpdate = new WaitForFixedUpdate();
            var size = collider.size;
            var offset = collider.offset;
            var startSize = new Vector2(size.x, size.y);
            var startOffset = new Vector2(offset.x, offset.y);
            var endSize = new Vector2(size.x, distance);
            var endOffset = new Vector2(0f, -distance / 2);
            for (var t = 0f; t < travelTime; t += Time.fixedDeltaTime)
            {
                collider.size = Vector2.Lerp(startSize, endSize, t/( 1.5f * travelTime));
                collider.offset = Vector2.Lerp(startOffset, endOffset, t/travelTime);
                yield return fixedUpdate;
            }
            collider.size = endSize;
            collider.offset = endOffset;
            rb.velocity = Vector2.zero;
            yield return new WaitForSeconds(ps.main.duration * .75f);
            collider.enabled = false;
            yield return new WaitForSeconds(2f);
            Destroy(gameObject);
        }
    }
}