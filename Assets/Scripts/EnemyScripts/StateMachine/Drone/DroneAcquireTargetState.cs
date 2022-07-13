using System;
using EntityStatsScripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EnemyScripts.Drone
{
    public class DroneAcquireTargetState : AcquireTargetState
    {
        private Strafe _strafeSteering;
        private IFire enemyShoot;


        protected override void Awake()
        {
            base.Awake();
            var parent = transform.parent;
            _strafeSteering = parent.GetComponentInChildren<Strafe>();
            enemyShoot = parent.GetComponentInChildren<IFire>();
        }


        private void PickDirection()
        {
            _strafeSteering.direction = Random.Range(0, 2) > 0 ? 1 : -1;
        }

        protected override Type TargetAcquired()
        {
            enemyShoot.CanShoot = true;
            return base.TargetAcquired();
        }

        protected override void SwitchState(BaseState state)
        {
            base.SwitchState(state);
            if (state.GetType() == GetType())
            {
                enemyShoot.CanShoot = false;
                PickDirection();
            }
            else
                enemyShoot.CanShoot = true;
        }
    }
}