using System.Collections;
using EntityStatsScripts;
using UnityEngine;

namespace EnemyScripts.Drone
{
    public class DroneShootTargetState : ShootTargetState
    {
        [SerializeField] private EnemyHealth health;
        [SerializeField] private BehaviourWeight[] enragedWeights;
        [SerializeField] private float enragedAcc;
        [SerializeField] private float enragedSpeed;
        [SerializeField] private float enrageDuration;
        private float _currEnragedDuration;
        private Coroutine _enrageRoutine;
        

        private bool _enraged;

        protected override void Awake()
        {
            health.onTakeDamage += Enrage;
            base.Awake();
        }
        
        protected void OnDestroy()
        {
            health.onTakeDamage -= Enrage;
        }
        
        private void Enrage()
        {
            _currEnragedDuration = enrageDuration;
            if (_enrageRoutine == null) 
                _enrageRoutine = StartCoroutine(EnrageForDuration());
        }

        protected override void SwitchState(BaseState state)
        {
            if (_enrageRoutine == null)
                base.SwitchState(state);
            else
            {
                foreach (var ew in enragedWeights)
                    ew.behaviour.weight = ew.weight;
                _steeringController.acceleration = enragedAcc;
                _steeringController.speed = enragedSpeed;
            }
        }

        private IEnumerator EnrageForDuration()
        {
            foreach (var ew in enragedWeights)
                ew.behaviour.weight = ew.weight;
            _steeringController.acceleration = enragedAcc;
            _steeringController.speed = enragedSpeed;
            
            while (_currEnragedDuration >= 0) {
                _currEnragedDuration -= Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            
            foreach (var bw in behaviourWeights)
                bw.behaviour.weight = bw.weight;
            _steeringController.acceleration = acceleration;
            _steeringController.speed = speed;
            _enrageRoutine = null;
        }
        
    }
}