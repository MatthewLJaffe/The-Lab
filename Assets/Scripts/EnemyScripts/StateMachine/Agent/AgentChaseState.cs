using System;
using UnityEngine;

namespace EnemyScripts.Agent
{
    public class AgentChaseState : DistanceBasedState
    {
        [SerializeField] private HumanAnimator moveAnimator;

        public override Type Tick()
        {
            moveAnimator.UpdateAnimationAiming();
            return base.Tick();
        }
    }
}