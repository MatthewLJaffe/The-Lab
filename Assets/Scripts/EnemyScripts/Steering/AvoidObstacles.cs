using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EnemyScripts 
{
    public class AvoidObstacles : SteeringBehaviour
    {
        [SerializeField] private float avoidThreshold = 1f;
        [SerializeField] private float raycastDistance;
        [SerializeField] private float aheadComponent;
        [SerializeField] private float maxAheadDistance;
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
            var raycastHits = RaycastHits(steeringWeights.Keys.Count, collider.transform.position);
            var steeringDirections = steeringWeights.Keys.ToList();
            //assign weights to directions with a direct hit
            foreach (var hit in raycastHits)
            {
                steeringDirections.Remove(hit.Key);
                var hitDistance = Vector2.Distance(hit.Value.point, transform.position);
                var distanceWeight =  1f - hitDistance / raycastDistance;
                var layerMultiplier = _layerWeightDict[LayerMask.LayerToName(hit.Value.collider.gameObject.layer)];
                var addedWeight = Mathf.Clamp(weight * distanceWeight * -avoidMultiplier * layerMultiplier, -1 ,1);
                steeringWeights[hit.Key] += addedWeight;
            }
            //assign weights to all other directions
            foreach (var dir in steeringDirections)
            {
                var inAvoidThreshold = false;
                //check for hits within avoid threshold
                foreach (var hit in raycastHits.Values)
                {
                    var hitDir = hit.point - (Vector2) transform.position;
                    var dot = Vector2.Dot(dir.normalized, hitDir.normalized);
                    if (dot < avoidThreshold) continue;
                    inAvoidThreshold = true;
                    dot *= avoidMultiplier;
                    var distanceWeight = 1f - hitDir.magnitude / raycastDistance;
                    var layerMultiplier = _layerWeightDict[LayerMask.LayerToName(hit.collider.gameObject.layer)];
                    var addedWeight = Mathf.Clamp(weight * -dot * distanceWeight * layerMultiplier, -1, 1);
                    steeringWeights[dir] += addedWeight;
                }
                if (inAvoidThreshold) continue;
                //See how far direction is from hit
                var aheadHit = Physics2D.Raycast((Vector2)collider.transform.position + collider.offset + dir.normalized * (collider.radius + .1f), dir,
                    maxAheadDistance, _avoidLayers);
                if (aheadHit)
                {
                    var aheadRange = maxAheadDistance - raycastDistance;
                    var adjustedAheadDist = Vector2.Distance(aheadHit.point, transform.position) - raycastDistance;
                    steeringWeights[dir] += (adjustedAheadDist - aheadRange / 2) / (aheadRange / 2) * aheadComponent;
                }
                else
                    steeringWeights[dir] += aheadComponent;

            }

        }

        private Dictionary<Vector2, RaycastHit2D> RaycastHits(int numCasts, Vector3 pos)
        {
            var raycastHits = new Dictionary<Vector2, RaycastHit2D>();
            for (var theta = 0f; theta < 360f; theta += 360f / numCasts)
            {
                var dir = new Vector2(Mathf.Cos(Mathf.Deg2Rad * theta), Mathf.Sin(Mathf.Deg2Rad * theta)).normalized;
                var hit = Physics2D.Raycast(
                    (Vector2) pos + collider.offset + dir.normalized * (collider.radius + .1f), dir,
                    raycastDistance, _avoidLayers);
                if (hit)
                    raycastHits.Add(dir, hit);
                if (debug)
                    Debug.DrawRay((Vector2)pos + collider.offset + dir.normalized * (collider.radius + .1f), 
                    dir.normalized * raycastDistance, Color.magenta, Time.fixedDeltaTime);
            }
            return raycastHits;
        }
    }
}
