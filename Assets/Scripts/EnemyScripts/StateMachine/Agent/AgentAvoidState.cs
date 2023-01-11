using System;
using UnityEngine;

namespace EnemyScripts.Agent
{
    /// <summary>
    /// state used so steer agent npc away from player
    /// requests agent animation
    /// </summary>
    public class AgentAvoidState : DistanceBasedState
    {
        [SerializeField] private HumanAnimator moveAnimator;

        public override Type Tick()
        {
            moveAnimator.UpdateAnimationAiming();
            return base.Tick();
        }
    }
}