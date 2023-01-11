using System;
using UnityEngine;
using UnityEngine.Events;

namespace General
{
    /// <summary>
    /// used to handle mouse entering worldspace object
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class MouseEvent : MonoBehaviour
    {
        public UnityEvent onMouseEnter;
        public UnityEvent onMouseExit;

        private void OnMouseEnter()
        {
            onMouseEnter.Invoke();
        }

        private void OnMouseExit()
        {
            onMouseExit.Invoke();
        }
    }
}