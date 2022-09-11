using System;
using EnemyScripts.BeegZombie;
using UnityEngine;

namespace EnemyScripts
{
    public class BeegZombieStompAttack : BeegZombieAttackState
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private BeegZombieAnimator animator;
        [SerializeField] private SteeringController steeringController;
        [SerializeField] private BaseState trackTargetState;
        [SerializeField] private float attackDuration;
        [SerializeField] private GameObject shockwave;
        [SerializeField] private Transform footPos;
        private float _currTime;

        public override Type Tick()
        {
            if (_currTime > attackDuration)
            {
                steeringController.enabled = true;
                return trackTargetState.GetType();
            }
            _currTime += Time.deltaTime;
            return null;
        }

        protected override void SwitchState(BaseState state)
        {
            base.SwitchState(state);
            if (state == this)
            {
                _currTime = 0;
                steeringController.enabled = false;
                rb.velocity = Vector2.zero;
                animator.currAttack = this;
                animator.PlayStomp();
            }
        }

        public void SpawnShockwave()
        {
            Instantiate(shockwave, footPos.position, Quaternion.identity);
        }
    }
}