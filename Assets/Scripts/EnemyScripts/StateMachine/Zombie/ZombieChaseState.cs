using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EnemyScripts.Zombie
{
    public class ZombieChaseState : DistanceBasedState
    {
        [SerializeField] private float minDashDistance;
        [SerializeField] private float strafeCooldown;
        private float _currentStrafeCooldown;
        [SerializeField] private float strafeChancePerSecond;
        private float _strafeChancePerFrame;

        protected override void Awake()
        {
            base.Awake();
            _steeringController = GetComponentInParent<SteeringController>();
            _strafeChancePerFrame = strafeChancePerSecond / (1f / Time.deltaTime);
        }



        public override Type Tick()
        {
            if (!enemy.target)
                return typeof(ZombieIdleState);
            
            var distanceType = base.Tick();
            if (distanceType != null)
                return distanceType;
            
            if (_currentStrafeCooldown <= 0 && Random.Range(0f, 1f) <= _strafeChancePerFrame
                && Vector2.Distance(enemy.target.position, transform.position) > minDashDistance)
            {
                _currentStrafeCooldown = strafeCooldown;
                return typeof(ZombieStrafeState);
            }
            _currentStrafeCooldown -= Time.deltaTime;
            return null;
        }
    }
}