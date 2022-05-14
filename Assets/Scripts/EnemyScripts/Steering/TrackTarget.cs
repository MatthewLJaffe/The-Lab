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
            foreach (var dir in steeringWeights.Keys.ToList())
            {
                var dot = Vector2.Dot(((Vector2)_enemy.target.position - (Vector2)transform.position).normalized, dir);
                steeringWeights[dir] += weight * dot;
            }
        }
    }
}
