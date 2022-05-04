using System;
using EnemyScripts.Zombie;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EnemyScripts
{
    public class ZombieStrafeState : BaseState
    {
        [SerializeField] private float strafeTime;
        private float _currentTime;
        private Strafe _strafeSteering;
        private Enemy _enemy;

        protected override void Awake()
        {
            var parent = transform.parent;
            base.Awake();
            _strafeSteering = parent.GetComponentInChildren<Strafe>();
            _currentTime = strafeTime;
        }

        private void Start()
        {
            _enemy = GetComponentInParent<Enemy>();
        }

        public override Type Tick()
        {
            if (_currentTime <= 0) {
                _currentTime = strafeTime;
                return typeof(ZombieChaseState);
            }
            //do at the beginning of the state
            if (Math.Abs(_currentTime - strafeTime) < Time.deltaTime) 
            {
                //determine direction to dash without collision if there is none go back to chase
                _strafeSteering.direction = PickDirection();
                if (_strafeSteering.direction == 0)
                    return typeof(ZombieChaseState);
            }
            _currentTime -= Time.deltaTime;
            return null;
        }

        private int PickDirection()
        {
            int pickedDirection = Random.Range(0, 2) > 0 ? 1 : -1;
            //if the direction that we want will result in a collision pick the other direction
            if (CanDashInDirection(pickedDirection)) return pickedDirection;
            pickedDirection *= -1;
            return CanDashInDirection(pickedDirection) ? pickedDirection : 0;
        }
        
        private bool CanDashInDirection(int direction)
        {
            //the direction we want to dash in
            Vector2 dashDir = Quaternion.Euler(0, 0, direction * _strafeSteering.AngleFromPlayer) *
                              (_enemy.target.position - transform.position).normalized;
            var hits = Physics2D.RaycastAll(transform.position, dashDir, strafeTime * speed, 
                LayerMask.GetMask("Default", "Enemy", "Block", "Spikes"));
            return hits.Length <= 1;
        }
    }
}