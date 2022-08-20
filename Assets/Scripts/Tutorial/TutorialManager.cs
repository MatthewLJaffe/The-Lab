using System;
using UnityEngine;

namespace Tutorial
{
    [CreateAssetMenu(fileName = "TutorialManager")]
    public class TutorialManager : ScriptableObject
    {
        [SerializeField] private int _actionsCompleted;
        public Action<int> actionsUpdate = delegate {  };

        public int ActionsCompleted
        {
            get => _actionsCompleted;
            set
            {
                _actionsCompleted = value;
                actionsUpdate.Invoke(_actionsCompleted);
            }
        }

        public void ResetActions()
        {
            _actionsCompleted = 0;
        }

        public void IncrementActionsCompleted()
        {
            ActionsCompleted++;
        }
    }
}