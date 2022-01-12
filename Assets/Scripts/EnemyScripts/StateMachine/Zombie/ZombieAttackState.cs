using System;
using UnityEngine;

namespace EnemyScripts.Zombie
{
    public class ZombieAttackState: BaseState
    {
        [SerializeField] private float attackRange;
        private Enemy _enemy;

        private void Start()
        {
            _enemy = GetComponentInParent<Enemy>();
            _steeringController = GetComponentInParent<SteeringController>();
        }

        public override Type Tick()
        {
            if (!_enemy.target || Vector2.Distance(transform.position, _enemy.target.position) > attackRange)
                return typeof(ZombieChaseState);
            return null;
        }
    }
}