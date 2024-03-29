﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyScripts
{
    /// <summary>
    /// base class used by steering controller to compute weights for different potential movement directions
    /// </summary>
    public abstract class SteeringBehaviour : MonoBehaviour
    {
        public float weight;
        [SerializeField] protected float avoidMultiplier;
        public abstract void AdjustWeights(Dictionary<Vector2, float> steeringWeights);
        
    }
}
