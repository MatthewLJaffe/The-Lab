using System;
using EntityStatsScripts.Effects;
using UnityEngine;
using UnityEngine.Events;

namespace EntityStatsScripts
{
    public class SteroidVisual : MonoBehaviour
    {
        [SerializeField] private SteroidEffect steroidEffect;
        public UnityEvent onSteroidStart;
        public UnityEvent onSteroidEnd;

        private void Awake()
        {
            steroidEffect.steroidEffectStart += () => onSteroidStart.Invoke();
            steroidEffect.steroidEffectEnd += () => onSteroidEnd.Invoke();
        }
    }
}