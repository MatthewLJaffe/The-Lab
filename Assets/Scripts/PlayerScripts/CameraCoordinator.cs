using System;
using UnityEngine;

namespace PlayerScripts
{
    public class CameraCoordinator : MonoBehaviour
    {
        [SerializeField] private Camera rotate;
        [SerializeField] private Camera main;
        [SerializeField] private Camera background;
        private float _currentSize;

        private void Start() {
            rotate.orthographicSize = main.orthographicSize;
            _currentSize = main.orthographicSize;
        }

        private void FixedUpdate()
        {
            if (Math.Abs(_currentSize - main.orthographicSize) > .01f) {
                rotate.orthographicSize = main.orthographicSize;
                _currentSize = main.orthographicSize;
            }
        }


    }
}
