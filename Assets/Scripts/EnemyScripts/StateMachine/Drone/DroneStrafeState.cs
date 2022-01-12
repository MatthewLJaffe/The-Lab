using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EnemyScripts.Drone
{
    [RequireComponent(typeof(Collider2D))]
    public class DroneStrafeState : BaseState
    {
        [SerializeField] private float avoidDistance;
        [SerializeField] private float chaseDistance;
        private Strafe _strafeSteering;
        private Enemy _enemy;
        
        
        protected override void Awake()
        {
            var parent = transform.parent;
            base.Awake();
            _strafeSteering = parent.GetComponentInChildren<Strafe>();
            _enemy = GetComponentInParent<Enemy>();
        }
        
        public override Type Tick()
        {
            if (!_enemy.target)
                return typeof(DroneIdleState);
            var distance = Vector2.Distance(_enemy.target.position, transform.position);
            if (distance < avoidDistance)
                return typeof(DroneAvoidState);
            if (distance > chaseDistance)
                return typeof(DroneChaseState);
            return null;
        }

        protected override void SwitchState(BaseState state)
        {
            base.SwitchState(state);
            if (state != this) return;
            PickDirection();
        }
        
        private void PickDirection()
        {
            _strafeSteering.direction = Random.Range(0, 2) > 0 ? 1 : -1;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject != transform.parent.gameObject) {
                _strafeSteering.direction *= -1;
            }
        }
    }
}