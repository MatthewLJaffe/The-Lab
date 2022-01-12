using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EnemyScripts
{
    public class RoamAround : SteeringBehaviour
    {
        [SerializeField] private float changeDirTime;
        private float _currentDirTime;
        private Vector2 _currentDir;
        private float _rayCastDistance = 4f;
        public override void AdjustWeights(Dictionary<Vector2, float> steeringWeights)
        {
            _currentDirTime -= Time.deltaTime;
            if (_currentDirTime <= 0) {
                _currentDir = GetDirection();
                _currentDirTime = changeDirTime;
            }
            
            foreach (var dir in steeringWeights.Keys.ToList()) {
                float weightChange = weight * Vector2.Dot(dir, _currentDir);
                weightChange = weightChange < 0 ? weightChange * avoidMultiplier : weightChange;
                steeringWeights[dir] += weightChange;
            }
        }

        private Vector2 GetDirection()
        {
            for (int i = 0; i < 20; i ++)
            {
                var newDir = new Vector2(Random.Range(-10f, 10f), Random.Range(-10f, 10f)).normalized;
                var hit = Physics2D.Raycast((Vector2) transform.position, newDir, _rayCastDistance, LayerMask.GetMask("Default"));
                if (hit.collider == null) return newDir;
            }
            return Vector2.zero;
        }
    }
}