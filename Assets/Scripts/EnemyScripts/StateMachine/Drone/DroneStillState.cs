using System;
using EntityStatsScripts;
using UnityEngine;

namespace EnemyScripts.Drone
{
    public class DroneStillState : BaseState
    {
        [SerializeField] private float avoidDistance;
        [SerializeField] private float chaseDistance;
        private Enemy _enemy;
        private bool _takenDamage;

        protected override void Awake()
        {
            base.Awake();
            _enemy = GetComponentInParent<Enemy>();
            _steeringController = GetComponentInParent<SteeringController>();
            GetComponentInParent<EnemyHealth>().onTakeDamage += () => _takenDamage = true;
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
            if (_takenDamage)
                return typeof(DroneStrafeState);
            return null;
        }

        protected override void SwitchState(BaseState state)
        {
            if (state != this) return;
            base.SwitchState(state);
            _takenDamage = false;
        }
    }
}