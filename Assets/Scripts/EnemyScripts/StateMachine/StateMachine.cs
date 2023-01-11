using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EnemyScripts
{
    /// <summary>
    /// executes behaviour and transitions of states
    /// </summary>
    public class StateMachine: MonoBehaviour
    {
        public Action<BaseState> onStateChange = delegate {  };
        private Dictionary<Type, BaseState> _availableStates;
        [SerializeField] private bool debug;
        public BaseState CurrentState { get; private set; }

        private void Awake()
        {
            SetStates();
            CurrentState = _availableStates.Values.First();
        }

        private void Start()
        {
            onStateChange(CurrentState);
        }

        //cache states
        private void SetStates()
        {
            _availableStates = new Dictionary<Type, BaseState>();
            foreach (var state in transform.Find("States").GetComponents<BaseState>())
            {
                _availableStates.Add(state.GetType(), state);
            }
        }

        private void FixedUpdate()
        {
            //executes current state and returns new state
            var nextState = CurrentState.GetState();
            
            //null means keep current state
            if (nextState != null && nextState != CurrentState.GetType())
            {
                if (debug)
                    Debug.Log("switching to " + nextState);
                SwitchStates(nextState);
            }
        }

        //update current state and invoke switch state event
        private void SwitchStates(Type state)
        {
            CurrentState = _availableStates[state];
            onStateChange(CurrentState);
        }
    }
}