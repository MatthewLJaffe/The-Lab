using System;
using System.Collections;
using UnityEngine;

namespace EnemyScripts.Zombie
{
    public class ZombieAcquireTargetState : AcquireTargetState
    {
        [SerializeField] private float fireCooldown;

        private Coroutine _waitCooldownRoutine;
        
        protected override Type TargetAcquired()
        {
            if (_waitCooldownRoutine != null) 
                return null;
            _waitCooldownRoutine = StartCoroutine(WaitForCoolDown());
            return targetAcquiredState.GetType();
        }
        
        private IEnumerator WaitForCoolDown()
        {
            yield return new WaitForSeconds(fireCooldown);
            _waitCooldownRoutine = null;
        }

    }
}