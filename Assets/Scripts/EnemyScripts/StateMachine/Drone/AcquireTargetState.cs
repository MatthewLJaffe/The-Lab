using System;
using UnityEngine;
using Random = UnityEngine.Random;


namespace EnemyScripts.Drone
{
    public class AcquireTargetState : DistanceBasedState
    {
        [SerializeField] private BaseState targetAcquiredState;
        private Strafe _strafeSteering;
        private EnemyShoot _enemyShoot;

        protected override void Awake()
        {
            base.Awake();
            var parent = transform.parent;
            _strafeSteering = parent.GetComponentInChildren<Strafe>();
            _enemyShoot = parent.GetComponentInChildren<EnemyShoot>();
        }

        public override Type Tick()
        {
            var distanceType = base.Tick();
            if (distanceType != null)
            {
                return distanceType;
            }
            Vector2 position = transform.position;
            var hit = Physics2D.BoxCast(position, new Vector2(.15f, 1f),
                0, (Vector2) enemy.target.position - position, farRange, Physics2D.GetLayerCollisionMask(LayerMask.NameToLayer("Enemy Bullet")));
            if (hit.transform == enemy.target) {
                _enemyShoot.CanShoot = true;
                return targetAcquiredState.GetType();
            }
            return null;
        }

        protected override void SwitchState(BaseState state)
        {
            base.SwitchState(state);
            if (state.GetType() != targetAcquiredState.GetType())
                _enemyShoot.CanShoot = false;
            if (state.GetType() != GetType()) return;
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