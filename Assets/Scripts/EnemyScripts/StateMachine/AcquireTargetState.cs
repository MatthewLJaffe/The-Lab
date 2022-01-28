using System;
using UnityEngine;


namespace EnemyScripts
{
    public class AcquireTargetState : DistanceBasedState
    {
        [SerializeField] protected BaseState targetAcquiredState;
        [SerializeField] protected BaseState targetLostState;
        
        public override Type Tick()
        {
            var distanceType = base.Tick();
            if (distanceType != null) {
                return distanceType;
            }
            Vector2 position = transform.position;
            var hit = Physics2D.BoxCast(position, new Vector2(.15f, 1f),
                0, (Vector2) enemy.target.position - position, farRange, Physics2D.GetLayerCollisionMask(LayerMask.NameToLayer("Enemy Bullet")));
            if (hit.transform == enemy.target) {
                return TargetAcquired();
            }
            return targetLostState ? targetLostState.GetType() : null;
        }
        
        /// <summary>
        /// Returned by Tick when there is a clear shot on the target
        /// </summary>
        /// <returns>The next state in the state machine</returns>
        protected virtual Type TargetAcquired()
        {
            return targetAcquiredState.GetType();
        }
    }
}