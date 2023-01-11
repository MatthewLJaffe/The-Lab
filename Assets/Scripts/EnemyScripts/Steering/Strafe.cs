using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EnemyScripts
{
    /// <summary>
    /// weights moving at certain angle from player direction positively
    /// </summary>
    public class Strafe : SteeringBehaviour
    {
        //determines if dashing in positive or negative angle form player
        [HideInInspector] public int direction;
        public float AngleFromPlayer => angleFromPlayer;
        [SerializeField] private float angleFromPlayer;
        private Enemy _enemy;
        
        private void Start()
        {
            _enemy = GetComponentInParent<Enemy>();
        }

        public override void AdjustWeights(Dictionary<Vector2, float> steeringWeights)
        {
            if (weight == 0) return;
            foreach (var dir in steeringWeights.Keys.ToList()) 
            {
                float theta = (Vector2.SignedAngle((_enemy.target.transform.position - transform.position).normalized, dir)) * Mathf.Deg2Rad;
                //shaping function weighting angle from player as 1
                theta += (90 - angleFromPlayer) * direction * Mathf.Deg2Rad;
                float priority = Mathf.Sin(theta);
                steeringWeights[dir] += weight * direction * priority;
            }
        }
        
    }
}