using System;
using UnityEngine;

namespace EnemyScripts.Drone
{
    public class DroneIdleState : BaseState
    {
        [SerializeField] private float aggroRange;
        private Enemy _enemy;
        private EnemyShoot _enemyShoot;

        protected override void Awake()
        {
            base.Awake();
            _steeringController = GetComponentInParent<SteeringController>();
            _enemyShoot = transform.parent.GetComponentInChildren<EnemyShoot>();
            _enemy = GetComponentInParent<Enemy>();
        }

        public override Type Tick()
        {
            if (!_enemy.EnteredRoom || !_enemy.target || 
                Vector2.Distance(_enemy.target.position, transform.position) >= aggroRange)
                return null;
            return typeof(DroneChaseState);
        }

        protected override void SwitchState(BaseState state)
        {
            if (state != this) return;
            base.SwitchState(state);
            _enemyShoot.CanShoot = false;
        }
    }
}