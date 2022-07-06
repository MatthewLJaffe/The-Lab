using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace General
{
    public class TimedAction : MonoBehaviour
    {
        [SerializeField] private bool beginOnStart;
        [SerializeField] private float time;
        [SerializeField] private UnityEvent action;

        private void Start()
        {
            if (beginOnStart)
                StartCoroutine(Timer());
        }

        public void StartTimer()
        {
            StartCoroutine(Timer());
        }

        private IEnumerator Timer()
        {
            yield return new WaitForSeconds(time);
            action.Invoke();
        }
    }
}