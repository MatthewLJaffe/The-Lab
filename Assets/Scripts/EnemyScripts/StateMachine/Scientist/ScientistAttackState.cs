using System;
using UnityEngine;

namespace EnemyScripts.Scientist
{
    public class ScientistAttackState : BaseState
    {
        [SerializeField] private BaseState runState;
        [SerializeField] private float maxRunDistance;
        [SerializeField] private ScientistFindCoverState findCoverState;
        [SerializeField] private BaseState chaseState;
        [SerializeField] private float minChaseDistance;
        [SerializeField] private HumanAnimator humanAnimator;
        private Enemy _enemy;
        private Rigidbody2D _rb;

        protected override void Awake()
        {
            _enemy = GetComponentInParent<Enemy>();
            _rb = GetComponentInParent<Rigidbody2D>();
            base.Awake();
        }
        
        public override Type Tick()
        {
            humanAnimator.UpdateAnimationAiming();
            var distance = Vector2.Distance(_enemy.target.position, _enemy.transform.position);
            if (distance > minChaseDistance)
                return chaseState.GetType();
            if (!findCoverState.InCover())
            {
                if (distance < maxRunDistance)
                    return runState.GetType();
                return findCoverState.GetType();
            }
            return null;
        }

        protected override void SwitchState(BaseState state)
        {
            humanAnimator.idle = state == this;
            _rb.velocity = Vector2.zero;
            base.SwitchState(state);
        }
    }
}