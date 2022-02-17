using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EnemyScripts
{
    public class RoamAround : SteeringBehaviour
    {
        [SerializeField] private float changeDirTime;
        private float _currTime;
        [SerializeField] private float farDistance;
        [SerializeField] private float maxOffsetAngle;
        private float _currentDirTime;
        private Vector2 _currentDir;
        private Vector2 _spawnPos;

        protected void Start()
        {
            _spawnPos = transform.parent.GetComponent<Enemy>().SpawnPos;
        }

        public override void AdjustWeights(Dictionary<Vector2, float> steeringWeights)
        {
            if (Vector2.Distance(_spawnPos, transform.position) > farDistance)
            {
                _currTime = 0;
                GetDirection();
            }
            if (_currTime > changeDirTime)
            {
                _currTime = 0;
                GetDirection();
            }
            
            foreach (var dir in steeringWeights.Keys.ToList()) {
                float weightChange = weight * Vector2.Dot(dir, _currentDir);
                weightChange = weightChange < 0 ? weightChange * avoidMultiplier : weightChange;
                steeringWeights[dir] += weightChange;
            }
            _currTime += Time.fixedDeltaTime;
        }

        private void GetDirection()
        {
            var generalDir = _spawnPos - (Vector2) transform.position;
            var offsetAngle = Random.Range(-maxOffsetAngle, maxOffsetAngle);
            _currentDir = Quaternion.Euler(0, 0, offsetAngle) * generalDir;
        }
    }
}