using System;
using UnityEngine;

namespace EnemyScripts.Drone
{
    public class ShootTargetState : DistanceBasedState
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Transform aimPoint;

        protected override void Awake()
        {
            base.Awake();
            if (!lineRenderer){}
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer)
            {
                lineRenderer.positionCount = 2;
                lineRenderer.enabled = false;
            }
        }

        public override Type Tick()
        {
            var distanceBasedState =  base.Tick();
            if (distanceBasedState != null)
                return distanceBasedState;
            Vector2 position = transform.position;
            var hit = Physics2D.Raycast(position, (Vector2)enemy.target.position - position,
                farRange, Physics2D.GetLayerCollisionMask(LayerMask.NameToLayer("Enemy Bullet")));
            if (lineRenderer) {
                lineRenderer.SetPosition(0, aimPoint.position);
                lineRenderer.SetPosition(1, enemy.target.position);
            }
            if (hit.transform == enemy.target) {
                return null;
            }
            return typeof(AcquireTargetState);
        }

        protected override void SwitchState(BaseState state)
        {
            base.SwitchState(state);
            if (!lineRenderer) return;
            lineRenderer.enabled = state.GetType() == GetType();
            if (!lineRenderer.enabled)
            {
                lineRenderer.SetPosition(0, Vector3.zero);
                lineRenderer.SetPosition(1, Vector3.zero);
            }
        }
    }
}