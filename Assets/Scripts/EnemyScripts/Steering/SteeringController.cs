using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EnemyScripts
{
    public class SteeringController : MonoBehaviour
    {
        [SerializeField] private int scanFrames;
        public float speed;
        public float acceleration;
        [SerializeField] private int scanDirections;
        [SerializeField] private float changeDirThreshold;
        [SerializeField] private bool debug;
        [HideInInspector] public int frameCount;
        private Dictionary<Vector2, float> _steeringWeights;
        private Rigidbody2D _rb;
        private SteeringBehaviour[] _steeringBehaviours;
        private Vector2 _previousDir = Vector2.zero;
        private Vector2 _accDir = Vector2.zero;
        

        private void Awake()
        {
            frameCount = scanFrames;
            _rb = GetComponent<Rigidbody2D>();
            _steeringWeights = new Dictionary<Vector2, float>();
            for (float theta = 0; theta < 360f; theta += 360f / scanDirections) {
                _steeringWeights.Add
                    (new Vector2(Mathf.Cos(Mathf.Deg2Rad * theta), Mathf.Sin(Mathf.Deg2Rad * theta)).normalized, 0);
            }
            _steeringBehaviours = transform.Find("SteeringBehaviours").GetComponents<SteeringBehaviour>();
        }
        
        private void FixedUpdate()
        {
            if (speed < .1f && _rb.velocity.magnitude < .1f) {
                _rb.velocity = Vector2.zero;
                return;
            }
            if (_steeringBehaviours.Sum(sb => sb.weight) == 0) return;
            var velocity = _rb.velocity;
            if (frameCount >= scanFrames) 
            {
                frameCount = 0;
                _accDir = SteerDirection().normalized * speed - velocity;
                if (debug)
                    DebugSteeringWeights();
                foreach (var dir in _steeringWeights.Keys.ToList())
                    _steeringWeights[dir] = 0;
            }
            _rb.AddForce(_accDir * (Time.fixedDeltaTime * acceleration * _rb.mass), ForceMode2D.Impulse);
            if (_rb.velocity.magnitude > speed)
                _rb.velocity = _rb.velocity.normalized * speed;
            frameCount++;
        }

        private Vector2 SteerDirection()
        {
            foreach (var sb in _steeringBehaviours) {
                sb.AdjustWeights(_steeringWeights);
            }
            //returns the key with the max weight
            var selectedDir = _steeringWeights.Aggregate((next, largest) =>
                next.Value > largest.Value ? next : largest).Key;
            //only change steer direction from last time if selected dir is significantly more desirable
            if (_previousDir != Vector2.zero)
                selectedDir = _steeringWeights[selectedDir] - _steeringWeights[_previousDir] > changeDirThreshold ? selectedDir : _previousDir;
            _previousDir = selectedDir;
            return selectedDir;
        }

        private void DebugSteeringWeights()
        {
            foreach (var weighted in _steeringWeights)
            {
                var arrowColor = Color.green;
                if (weighted.Value < 0) 
                    arrowColor = Color.red;
                Debug.DrawRay(transform.position, weighted.Key.normalized * (Mathf.Abs(weighted.Value) * 2), arrowColor, Time.fixedDeltaTime * scanFrames, 
                    false);
            }
        }
    }
}