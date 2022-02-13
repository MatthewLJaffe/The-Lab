using System;
using UnityEngine;
using UnityEngine.Events;

namespace General
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]

    public class CollisionDetector : MonoBehaviour
    {
        public LayerMask layers;
        public UnityEvent collisionEnter;
        public UnityEvent collisionStay;
        public UnityEvent collisionExit;
        public UnityEvent triggerEnter;
        public UnityEvent triggerStay;
        public UnityEvent triggerExit;

        private void OnCollisionEnter2D(Collision2D other)
        {
            if ( layers != (layers | (1 << other.gameObject.layer))) return;
            collisionEnter.Invoke();
        }
        
        private void OnCollisionStay2D(Collision2D other)
        {            
            if ( layers != (layers | (1 << other.gameObject.layer))) return;
            collisionStay.Invoke();
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if ( layers != (layers | (1 << other.gameObject.layer))) return;
            collisionExit.Invoke();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ( layers != (layers | (1 << other.gameObject.layer))) return;
            triggerEnter.Invoke();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if ( layers != (layers | (1 << other.gameObject.layer))) return;
            triggerStay.Invoke();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if ( layers != (layers | (1 << other.gameObject.layer))) return;
            triggerExit.Invoke();
        }
    }
}