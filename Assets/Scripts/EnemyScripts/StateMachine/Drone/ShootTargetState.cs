using System;
using UnityEngine;

namespace EnemyScripts.Drone
{
    public class ShootTargetState : DistanceBasedState
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Transform aimPoint;
        [SerializeField] private BaseState targetLostState;

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
            if (aimPoint)
                position = aimPoint.position;
            var mask = Physics2D.GetLayerCollisionMask(LayerMask.NameToLayer("Enemy Bullet"));
            mask |= LayerMask.GetMask(LayerMask.LayerToName(enemy.target.gameObject.layer));
            var hit = Physics2D.BoxCast(position, new Vector2(.25f,.25f), 0, 
                (Vector2)enemy.target.position - position, farRange, mask);
            if (lineRenderer) {
                lineRenderer.SetPosition(0, aimPoint.position);
                lineRenderer.SetPosition(1, enemy.target.position);
            }
            if (hit.transform == enemy.target) {
                return null;
            }
            return targetLostState.GetType();
        }

        protected override void SwitchState(BaseState state)
        {
            base.SwitchState(state);
            if (!lineRenderer) return;
            lineRenderer.SetPosition(0, new Vector3(999, 999, 999));
            lineRenderer.SetPosition(1, new Vector3(999, 999, 999));
        }
    }
}