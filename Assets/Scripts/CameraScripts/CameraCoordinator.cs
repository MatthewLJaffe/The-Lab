using System;
using UnityEngine;

namespace CameraScripts
{
    public class CameraCoordinator : MonoBehaviour
    {
        [SerializeField] private Camera main;
        [SerializeField] private Camera background;
        public Camera miniMapCamera;
        private float _currentSize;

        private void Start() {
            _currentSize = main.orthographicSize;
        }


    }
}
