using System;
using UnityEngine;

namespace EnemyScripts.Scientist
{
    public class ScientistChaseState : DistanceBasedState
    {
        [SerializeField] private HumanAnimator moveAnimator;

        public override Type Tick()
        {
            moveAnimator.UpdateAnimationNoAim();
            return base.Tick();
        }
    }
}