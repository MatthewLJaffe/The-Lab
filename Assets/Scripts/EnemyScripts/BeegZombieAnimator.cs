using EnemyScripts.BeegZombie;
using UnityEngine;

namespace EnemyScripts
{
    /// <summary>
    /// animation controller for zombie boss
    /// </summary>
    public class BeegZombieAnimator : MonoBehaviour
    {
        [SerializeField] private BeegZombieChargeState chargeAttack;
        [SerializeField] private BeegZombieStompAttack stompAttack;
        [SerializeField] private BeegZombieColumnAttack columnAttack;
        [SerializeField] private string downAnim;
        [SerializeField] private string leftAnim;
        [SerializeField] private string rightAnim;
        [SerializeField] private string upAnim;
        [SerializeField] private string chargeDownAnim;
        [SerializeField] private string chargeLeftAnim;
        [SerializeField] private string chargeRightAnim;
        [SerializeField] private string chargeUpAnim;
        [SerializeField] private string stunnedAnimation;
        [SerializeField] private string stompAnim;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Animator anim;
        [SerializeField] private Vector2[] directions;
        public BeegZombieAttackState currAttack;

        
        public void UpdateWalkAnimation()
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
                anim.Play(leftAnim);
            }
            else
            {
                anim.Play(rightAnim);
            }
            
        }

        public void PlayChargeAnimation(Vector2 dir)
        {
            var maxDir = directions[0];
            for (var i = 1; i < directions.Length; i++)
            {
                if (Vector2.Dot(dir, maxDir) < Vector2.Dot(dir, directions[i]))
                    maxDir = directions[i];
            }
            if (maxDir == Vector2.up)
            {
                if (dir.x > 0)
                    anim.Play(chargeUpAnim);
                else
                    anim.Play(chargeLeftAnim);
            }
            else if (maxDir == Vector2.down)
            {
                anim.Play(chargeDownAnim);
            }
            else if (maxDir == Vector2.left)
            {
                anim.Play(chargeLeftAnim);
            }
            else
            {
                anim.Play(chargeRightAnim);
            }
        }

        public void RecomputeChargeDir()
        {
            chargeAttack.ComputeChargeDir();
        }

        public void StartCharge()
        {
            chargeAttack.charging = true;
        }
        
        public void PlayStunnedAnimation()
        {
            anim.Play(stunnedAnimation);
        }

        public void SpawnAttack()
        {
            if (currAttack == stompAttack)
                stompAttack.SpawnShockwave();
            else if (currAttack == columnAttack)
                columnAttack.SpawnColumn();
        }

        public void PlayStomp()
        {
            anim.Play(stompAnim);
        }
    }
    
}