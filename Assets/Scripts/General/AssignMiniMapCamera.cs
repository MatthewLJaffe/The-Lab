using System;
using CameraScripts;
using PlayerScripts;
using UnityEngine;

namespace General
{
    public class AssignMiniMapCamera : MonoBehaviour
    {
        private Camera _miniMapCamera;
        private Canvas _canvas;
        [SerializeField] private bool mainCamera;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
        }

        private void Start()
        {
            if (mainCamera)
            {
                _canvas.worldCamera = Camera.main;
                return;
            }
            _miniMapCamera = PlayerFind.instance.playerInstance.GetComponentInChildren<CameraCoordinator>().miniMapCamera;
            _canvas.worldCamera = _miniMapCamera;
        }
    }
}