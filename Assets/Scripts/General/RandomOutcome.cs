using System;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace General
{
    public class RandomOutcome : MonoBehaviour
    {
        [SerializeField] private OutcomeWeight[] possibilities;
        private void Start()
        {
            var rand = Random.Range(0f, 1f);
            foreach (var p in possibilities)
            {
                if (p.weight >= rand) {
                    p.outcome.Invoke();
                    return;
                }
                rand -= p.weight;
            }
        }

        [Serializable]
        private struct OutcomeWeight
        {
            public float weight;
            public UnityEvent outcome;
        }
    }
}