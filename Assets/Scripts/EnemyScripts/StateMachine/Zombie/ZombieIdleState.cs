using System;
using EnemyScripts.Zombie;
using UnityEngine;

namespace EnemyScripts
{
    public class ZombieIdleState : BaseState
    {
        [SerializeField] private float aggroRange;
        [SerializeField] private BaseState aggroState;
        private Enemy _enemy;

        protected override void Awake()
        {
            base.Awake();
            _steeringController = GetComponentInParent<SteeringController>();
            _enemy = GetComponentInParent<Enemy>();
        }

        public override Type Tick()
        {
            if (!_enemy.target || Vector2.Distance(_enemy.target.position, transform.position) >= aggroRange)
                return null;
            return aggroState.GetType();
        }
    }
}