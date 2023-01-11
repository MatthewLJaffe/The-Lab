using System;
using UnityEngine;

namespace EnemyScripts
{
    /// <summary>
    /// state that executes behaviour and changes state based on predefined distance thresholds
    /// </summary>
    public class DistanceBasedState : BaseState
    {
        [SerializeField] protected BaseState farState;
        [SerializeField] protected float farRange;
        [SerializeField] protected BaseState closeState;
        [SerializeField] protected float closeRange;
        protected Enemy enemy;

        protected override void Awake()
        {
            base.Awake();
            enemy = GetComponentInParent<Enemy>();
        }
        
        public override Type Tick()
        {
            if (Vector2.Distance(enemy.target.transform.position, transform.position) >= farRange && farState)
                return farState.GetType();
            if (Vector2.Distance(enemy.target.transform.position, transform.position) <= closeRange && closeState)
                return closeState.GetType();
            return null;
        }
    }
}