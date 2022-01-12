using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EnemyScripts
{
    public class StateMachine: MonoBehaviour
    {
        public Action<BaseState> OnStateChange = delegate {  };
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
            OnStateChange(CurrentState);
        }

        public void SetStates(Dictionary<Type, BaseState> states)
        {
            _availableStates = states;
        }

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
            var nextState = CurrentState.Tick();
            if (nextState != null && nextState != CurrentState.GetType())
            {
                if (debug)
                    Debug.Log("switching to " + nextState);
                SwitchStates(nextState);
            }
        }

        private void SwitchStates(Type state)
        {
            CurrentState = _availableStates[state];
            OnStateChange(CurrentState);
        }
    }
}