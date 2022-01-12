using System;
using UnityEngine;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu (fileName = "DiseaseToleranceEffect", menuName = "Effects/DiseaseToleranceEffect")]
    public class DiseaseToleranceEffect : Effect
    {
        public Action<float> OnDiseaseToleranceChange = delegate { }; //float is factor to multiply infection rate by
        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            OnDiseaseToleranceChange.Invoke( .25f + 3f / (2f * newStack + 4f));
        }
    }
}