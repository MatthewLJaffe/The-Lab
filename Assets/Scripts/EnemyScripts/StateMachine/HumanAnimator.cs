using PlayerScripts;
using UnityEngine;

namespace EnemyScripts
{
    /// <summary>
    /// animation controller for humanoid npcs
    /// </summary>
    public class HumanAnimator : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private string upAnim;
        [SerializeField] private string reverseUpAnim;
        [SerializeField] private string idleUpAnim;
        [SerializeField] private string downAnim;
        [SerializeField] private string reverseDownAnim;
        [SerializeField] private string idleDownAnim;
        [SerializeField] private string sidewaysAnim;
        [SerializeField] private string reverseSidewaysAnim;
        [SerializeField] private string idleSidewaysAnim;
        [SerializeField] private Animator anim;
        [SerializeField] private SpriteRenderer gunSr;
        [SerializeField] private ShootController enemyShoot;
        [SerializeField] private Transform shootPoint;
        [SerializeField] private Transform flippedShootPoint;
        [SerializeField] private Vector2[] directions;
        public bool idle;
        private Transform _playerTrans;

        private void Start()
        {
            _playerTrans = PlayerFind.instance.playerInstance.transform;
        }

        public void UpdateAnimationAiming()
        {
            if (!_playerTrans) return;
            var dir = _playerTrans.position - transform.position;
            if (idle)
            {
                var maxDir = directions[0];
                for (var i = 1; i < directions.Length; i++)
                {
                    if (Vector2.Dot(dir, maxDir) < Vector2.Dot(dir, directions[i]))
                        maxDir = directions[i];
                }
                if (maxDir == Vector2.up)
                {
                    anim.Play(idleUpAnim);
                }
                else if (maxDir == Vector2.down)
                {
                    anim.Play(idleDownAnim);
                }
                else if (maxDir == Vector2.left)
                {
                    anim.Play(idleSidewaysAnim);
                    transform.rotation = Quaternion.Euler(0,180,0);
                }
                else
                {
                    anim.Play(idleSidewaysAnim);
                    transform.rotation = Quaternion.identity;
                }
                return;
            }
            
            if (Mathf.Abs(dir.y) > Mathf.Abs(dir.x))
            {
                transform.rotation = Quaternion.Euler(0,0,0);
                if (dir.y > 0)
                    anim.Play(Vector2.Dot(Vector2.up, rb.velocity) > 0 ? upAnim : reverseUpAnim);
                else
                    anim.Play(Vector2.Dot(Vector2.down, rb.velocity) > 0 ? downAnim : reverseDownAnim);
            }
            else
            {
                if (dir.x > 0)
                {
                    anim.Play(Vector2.Dot(Vector2.right, rb.velocity) > 0 ? sidewaysAnim : reverseSidewaysAnim);
                    transform.rotation = Quaternion.Euler(0,0,0);

                }
                else
                {
                    anim.Play(Vector2.Dot(Vector2.left, rb.velocity) > 0 ? sidewaysAnim : reverseSidewaysAnim);
                    transform.rotation = Quaternion.Euler(0,180,0);
                }
            }
            if (!gunSr) return;
            if (dir.x < 0)
            {
                gunSr.flipX = true;
                enemyShoot.shootPoint = flippedShootPoint;
            }
            else
            {
                gunSr.flipX = false;
                enemyShoot.shootPoint = shootPoint;
            }
        }
        
        public void UpdateAnimationNoAim()
        {
            var dir = rb.velocity.normalized;

            var maxDir = directions[0];
            for (var i = 1; i < directions.Length; i++)
            {
                if (Vector2.Dot(dir, maxDir) < Vector2.Dot(dir, directions[i]))
                    maxDir = directions[i];
            }
            if (maxDir == Vector2.up)
            {
                anim.Play(upAnim);
            }
            else if (maxDir == Vector2.down)
            {
                anim.Play(downAnim);
            }
            else if (maxDir == Vector2.left)
            {
                anim.Play(sidewaysAnim);
                transform.rotation = Quaternion.Euler(0,180,0);
            }
            else
            {
                anim.Play(sidewaysAnim);
                transform.rotation = Quaternion.identity;
            }
            
        }
    }
}