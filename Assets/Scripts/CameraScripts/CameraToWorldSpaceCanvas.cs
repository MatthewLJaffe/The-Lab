using System;
using UnityEngine;

namespace CameraScripts
{
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