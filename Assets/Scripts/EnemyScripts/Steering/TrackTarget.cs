using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EnemyScripts
{
    public class TrackTarget : SteeringBehaviour
    {
        private Enemy _enemy;
        
        private void Awake()
        {
            _enemy = GetComponentInParent<Enemy>();
        }

        public override void AdjustWeights(Dictionary<Vector2, float> steeringWeights)
        {
            if (_enemy.target == null) return;
            foreach (var dir in steeringWeights.Keys.ToList()) {
                steeringWeights[dir] += weight * Vector2.Dot((_enemy.target.position - transform.position).normalized, dir);
            }
        }
    }
}
