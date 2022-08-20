using System;
using UnityEngine;
using UnityEngine.Events;

namespace Tutorial
{
    public class TutorialEvent : MonoBehaviour
    {
        [SerializeField] private int actionToFire;
        [SerializeField] private TutorialManager tutorialManager;
        public UnityEvent onActionReached;

        private void Awake()
        {
            tutorialManager.actionsUpdate += InvokeEvent;
        }

        private void OnDestroy()
        {
            tutorialManager.actionsUpdate -= InvokeEvent;
        }

        private void InvokeEvent(int actionsComplete)
        {
            if (actionsComplete == actionToFire)
                onActionReached.Invoke();
        }
    }
}