using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EnemyScripts.Zombie
{
    public class ZombieChaseState : BaseState
    {
        [SerializeField] private float attackRange;
        [SerializeField] private float aggroRange;
        [SerializeField] private float minDashDistance;
        [SerializeField] private float strafeCooldown;
        private float _currentStrafeCooldown;
        [SerializeField] private float strafeChancePerSecond;
        private float _strafeChancePerFrame;
        private Enemy _enemy;

        protected override void Awake()
        {
            base.Awake();
            _steeringController = GetComponentInParent<SteeringController>();
            _strafeChancePerFrame = strafeChancePerSecond / (1f / Time.deltaTime);
        }

        private void Start()
        {
            _enemy = GetComponentInParent<Enemy>();
        }

        public override Type Tick()
        {
            if (!_enemy.target || Vector2.Distance(_enemy.target.position, transform.position) > aggroRange)
                return typeof(ZombieIdleState);
            if (Vector2.Distance(_enemy.target.position, transform.position) < attackRange)
                return typeof(ZombieAttackState);
            if (_currentStrafeCooldown <= 0 && Random.Range(0f, 1f) <= _strafeChancePerFrame
                && Vector2.Distance(_enemy.target.position, transform.position) > minDashDistance)
            {
                _currentStrafeCooldown = strafeCooldown;
                return typeof(ZombieStrafeState);
            }
            _currentStrafeCooldown -= Time.deltaTime;
            return null;
        }
    }
}