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
            if (enragedWeights == null || enragedWeights.Length == 0) return;
            _currEnragedDuration = enrageDuration;
            if (_enrageRoutine == null) 
                _enrageRoutine = StartCoroutine(EnrageForDuration());
        }
        
        protected override void SwitchState(BaseState state)
        {
            if (state != this) return;
            if (_enrageRoutine == null || enragedWeights == null || enragedWeights.Length == 0)
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
            if (enragedWeights == null || enragedWeights.Length == 0) yield break;
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