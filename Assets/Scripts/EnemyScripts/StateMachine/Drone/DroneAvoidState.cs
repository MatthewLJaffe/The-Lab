using System;
using UnityEngine;

namespace EnemyScripts.Drone
{
    public class DroneAvoidState: BaseState
    {
        [SerializeField] private float farRange;
        [SerializeField] private BaseState farState;
        private Enemy _enemy;

        private void Start()
        {
            _enemy = GetComponentInParent<Enemy>();
            _steeringController = GetComponentInParent<SteeringController>();
        }

        public override Type Tick()
        {
            if (!_enemy.target)
                return typeof(DroneIdleState);
            if (Vector2.Distance(transform.position, _enemy.target.position) > farRange)
                return farState.GetType();
            return null;
        }
    }
}