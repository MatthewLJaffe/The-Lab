using System;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyScripts
{
    public abstract class SteeringBehaviour : MonoBehaviour
    {
        public float weight;
        [SerializeField] protected float avoidMultiplier;
        public abstract void AdjustWeights(Dictionary<Vector2, float> steeringWeights);
        
    }
}
