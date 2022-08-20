using System;
using UnityEngine;

namespace EnemyScripts.Scientist
{
    public class ScientistAvoidState : DistanceBasedState
    {
        [SerializeField] private HumanAnimator moveAnimator;

        public override Type Tick()
        {
            moveAnimator.UpdateAnimationNoAim();
            return base.Tick();
        }
    }
}