using System;
using UnityEngine;

namespace CameraScripts
{
    /// <summary>
    /// dynamically set worldspace camera of canvas at runtime
    /// </summary>
    public class CameraToWorldSpaceCanvas : MonoBehaviour
    {
        private Camera _camera;
        private void Start()
        {
            _camera = Camera.main;
            GetComponent<Canvas>().worldCamera = _camera;
        }
    }
}