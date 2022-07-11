using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EnemyScripts 
{
    public class AvoidObstacles : SteeringBehaviour
    {
        [SerializeField] private float avoidThreshold = 1f;
        [SerializeField] private float nonHitMultiplier = .5f;
        [SerializeField] private float raycastDistance;
        [SerializeField] private bool debug;
        [SerializeField] private CircleCollider2D collider;
        [SerializeField] private LayerWeight[] layerWeights;
        private LayerMask _avoidLayers;
        private Dictionary<string, float> _layerWeightDict;
        private RaycastHit2D[] _raycastResults;
        private Vector2 _colliderOffset;

        [System.Serializable]
        private struct LayerWeight
        {
            public string layer;
            public float weight;
        }

        private void Awake()
        {
            _colliderOffset = collider.offset;
            var layers = new string[layerWeights.Length];
            _raycastResults = new RaycastHit2D[10];
            _layerWeightDict = new Dictionary<string, float>();
            for (int i = 0; i < layerWeights.Length; i++) {
                layers[i] = layerWeights[i].layer;
                _layerWeightDict.Add(layerWeights[i].layer, layerWeights[i].weight);
            }
            _avoidLayers = LayerMask.GetMask(layers);
        }

        public override void AdjustWeights(Dictionary<Vector2, float> steeringWeights)
        {
            var raycastHits = RaycastHits(steeringWeights.Keys.Count);
            var steeringDirections = steeringWeights.Keys.ToList();
            //assign weights to directions with a direct hit
            foreach (var hit in raycastHits)
            {
                steeringDirections.Remove(hit.Key);
                var hitDistance = (hit.Value.point - (Vector2) transform.position).magnitude;
                var distanceWeight =  1f - hitDistance / raycastDistance;
                var layerMultiplier = _layerWeightDict[LayerMask.LayerToName(hit.Value.collider.gameObject.layer)];
                var addedWeight = Mathf.Clamp(weight * distanceWeight * -avoidMultiplier * layerMultiplier, -1 ,1);
                steeringWeights[hit.Key] += addedWeight;
            }
            //assign weights to all other directions
            //TODO create ahead component
            foreach (var dir in steeringDirections)
            {
                foreach (var hit in raycastHits.Values)
                {
                    var hitDir = hit.point - (Vector2) transform.position;
                    var dot = Vector2.Dot(dir.normalized, hitDir.normalized);
                    if (dot > avoidThreshold)
                        dot *= avoidMultiplier;
                    else
                        dot *= nonHitMultiplier;
                    var distanceWeight = 1f - hitDir.magnitude / raycastDistance;
                    var layerMultiplier = _layerWeightDict[LayerMask.LayerToName(hit.collider.gameObject.layer)];
                    var addedWeight = Mathf.Clamp(-dot * (1f / steeringWeights.Count) * distanceWeight * layerMultiplier, -1, 1);
                    steeringWeights[dir] += addedWeight;
                }
            }
        }

        private Dictionary<Vector2, RaycastHit2D> RaycastHits(int numCasts)
        {
            var raycastHits = new Dictionary<Vector2, RaycastHit2D>();
            for (var theta = 0f; theta < 360f; theta += 360f / numCasts)
            {
                var dir = new Vector2(Mathf.Cos(Mathf.Deg2Rad * theta), Mathf.Sin(Mathf.Deg2Rad * theta)).normalized;
                var hit = Physics2D.Raycast(
                    (Vector2) collider.gameObject.transform.position + collider.offset + dir.normalized * (collider.radius + .1f), dir,
                    raycastDistance, _avoidLayers);
                if (hit)
                    raycastHits.Add(dir, hit);
                if (debug)
                    Debug.DrawRay((Vector2) collider.gameObject.transform.position + collider.offset + dir.normalized * (collider.radius + .1f), 
                    dir.normalized * raycastDistance, Color.magenta, Time.fixedDeltaTime);
            }
            return raycastHits;
        }
    }
}
