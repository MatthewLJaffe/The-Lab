using System;
using UnityEngine;
using UnityEngine.Events;

namespace EnemyScripts
{
    /// <summary>
    /// base class for behaviour of state in state machine
    /// </summary>
    public abstract class BaseState : MonoBehaviour
    {
        [SerializeField] protected BehaviourWeight[] behaviourWeights;
        [SerializeField] protected float speed;
        [SerializeField] protected float acceleration;
        [HideInInspector] public BaseState OverrideState;
        protected SteeringController _steeringController;
        public UnityEvent onSwitchTo;
        
        protected virtual void Awake()
        {
            transform.GetComponentInParent<StateMachine>().onStateChange += SwitchState;
            _steeringController = GetComponentInParent<SteeringController>();
        }

        public virtual Type GetState()
        {
            if (OverrideState != null) {
                var state = OverrideState.GetType();
                OverrideState = null;
                return state;
            }
            return Tick();
        }
        
        //executes state behaviour
        public abstract Type Tick();

        protected virtual void SwitchState(BaseState state)
        {
            if (state != this) return;
            foreach (var bw in behaviourWeights)
                bw.behaviour.weight = bw.weight;
            _steeringController.acceleration = acceleration;
            _steeringController.speed = speed;
            onSwitchTo.Invoke();
        }

        //steering behaviour weights to use for steering controller
        [Serializable]
        protected struct BehaviourWeight
        {
            public SteeringBehaviour behaviour;
            public float weight;
        }
        
    }
}