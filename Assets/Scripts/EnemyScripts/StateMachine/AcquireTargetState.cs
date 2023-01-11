using System;
using UnityEngine;


namespace EnemyScripts
{
    /// <summary>
    /// state for raycasting to target for clear shot
    /// </summary>
    public class AcquireTargetState : DistanceBasedState
    {
        [SerializeField] protected BaseState targetAcquiredState;
        [SerializeField] protected BaseState targetLostState;
        [SerializeField] protected Transform shootPoint;
        [SerializeField] protected float bulledWidth = .25f;
        
        public override Type Tick()
        {
            var distanceType = base.Tick();
            if (distanceType != null) {
                return distanceType;
            }
            Vector2 position = shootPoint.position;
            var mask = Physics2D.GetLayerCollisionMask(LayerMask.NameToLayer("Enemy Bullet"));
            mask |= LayerMask.GetMask(LayerMask.LayerToName(enemy.target.gameObject.layer));
            var hit = Physics2D.BoxCast(position, new Vector2(bulledWidth,bulledWidth),0, 
                (Vector2) enemy.target.position - position, farRange, mask);
            if (hit.transform == enemy.target) {
                return TargetAcquired();
            }
            return targetLostState ? targetLostState.GetType() : null;
        }
        
        /// <summary>
        /// Returned by Tick when there is a clear shot on the target
        /// </summary>
        /// <returns>The next state in the state masachine</returns>
        protected virtual Type TargetAcquired()
        {
            return targetAcquiredState.GetType();
        }
    }
}