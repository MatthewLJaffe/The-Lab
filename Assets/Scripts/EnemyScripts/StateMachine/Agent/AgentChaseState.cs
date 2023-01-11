using System;
using UnityEngine;

namespace EnemyScripts.Agent
{
    /// <summary>
    /// state used so steer agent npc towards player
    /// requests agent animation
    /// </summary>
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