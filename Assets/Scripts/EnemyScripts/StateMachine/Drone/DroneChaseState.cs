using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace EnemyScripts.Drone
{
    public class DroneChaseState : BaseState
    {
        [SerializeField] private float closeDistance;
        [SerializeField] private float idleDistance;
        [SerializeField] private BaseState closeState;
        [SerializeField] private bool canShoot;

        private Enemy _enemy;
        private EnemyShoot _enemyShoot;

        private void Start()
        {
            _enemy = GetComponentInParent<Enemy>();
            _enemyShoot = transform.parent.GetComponentInChildren<EnemyShoot>();
        }
        public override Type Tick()
        {
            if (!_enemy.target)
                return typeof(DroneIdleState);
            var distance = Vector2.Distance(_enemy.target.position, transform.position);
            if (distance >= idleDistance)
                return typeof(DroneIdleState);
            if (distance <= closeDistance)
                return closeState.GetType();
            return null;
        }

        protected override void SwitchState(BaseState state)
        {
            if (state != this) return;
            base.SwitchState(state);
            _enemyShoot.CanShoot = canShoot;
        }
    }
}