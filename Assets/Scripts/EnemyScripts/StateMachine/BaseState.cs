using System;
using UnityEngine;

namespace EnemyScripts
{
    public abstract class BaseState : MonoBehaviour
    {
        [SerializeField] protected BehaviourWeight[] behaviourWeights;
        [SerializeField] protected float speed;
        [SerializeField] protected float acceleration;
        protected SteeringController _steeringController;
        
        protected virtual void Awake()
        {
            transform.GetComponentInParent<StateMachine>().onStateChange += SwitchState;
            _steeringController = GetComponentInParent<SteeringController>();
        }
        public abstract Type Tick();

        protected virtual void SwitchState(BaseState state)
        {
            if (state != this) return;
            foreach (var bw in behaviourWeights)
                bw.behaviour.weight = bw.weight;
            _steeringController.acceleration = acceleration;
            _steeringController.speed = speed;
        }

        [Serializable]
        protected struct BehaviourWeight
        {
            public SteeringBehaviour behaviour;
            public float weight;
        }
        
    }
}