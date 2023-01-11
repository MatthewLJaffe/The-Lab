using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace General
{
    /// <summary>
    /// activation and deactivation events on timer
    /// used by timed spikes
    /// </summary>
    public class CycleTimer : MonoBehaviour
    {
        [SerializeField] private float startOffset;
        [SerializeField] private float onTime;
        [SerializeField] private float offTime;
        public UnityEvent onActive;
        public UnityEvent onInactive;

        private void Start()
        {
            StartCoroutine(BeginOffset());
        }

        private IEnumerator BeginOffset()
        {
            onInactive.Invoke();
            yield return new WaitForSeconds(startOffset);
            StartCoroutine(Active());
        }

        private IEnumerator Active()
        {
            onActive.Invoke();
            yield return new WaitForSeconds(onTime);
            StartCoroutine(Inactive());
        }

        private IEnumerator Inactive()
        {
            onInactive.Invoke();
            yield return new WaitForSeconds(offTime);
            StartCoroutine(Active());
        }
    }
    
    
}