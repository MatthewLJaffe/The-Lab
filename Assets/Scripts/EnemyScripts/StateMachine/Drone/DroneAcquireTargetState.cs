using System;
using System.Collections;
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

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.transform.parent && LayerMask.LayerToName(col.transform.parent.gameObject.layer) == "Drone")
            {
                var otherStrafe = col.transform.parent.GetComponentInChildren<Strafe>();
                if (otherStrafe && otherStrafe.direction == _strafeSteering.direction)
                {
                    _strafeSteering.direction *= -1;
                }
            }
        }

        protected override Type TargetAcquired()
        {
            enemyShoot.CanShoot = true;
            return base.TargetAcquired();
        }
    }
    
}