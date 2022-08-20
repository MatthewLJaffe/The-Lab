using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EnemyScripts.Agent
{
    public class AgentCircleState : DistanceBasedState
    {
        [SerializeField] private HumanAnimator moveAnimator;
        [SerializeField] private Strafe strafe;

        public override Type Tick()
        {
            moveAnimator.UpdateAnimationAiming();
            return base.Tick();
        }

        protected override void SwitchState(BaseState state)
        {
            var rand = Random.Range(0, 2);
            strafe.direction = (int)Mathf.Pow(-1, rand);
            base.SwitchState(state);
        }
    }
}