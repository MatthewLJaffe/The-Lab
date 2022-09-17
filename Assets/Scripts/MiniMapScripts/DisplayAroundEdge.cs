using System;
using CameraScripts;
using PlayerScripts;
using UnityEngine;

namespace MiniMapScripts
{
    public class DisplayAroundEdge : MonoBehaviour
    {
        private Camera miniMapCamera;
        private Vector3 initialPos;
        [SerializeField] private Vector2 offset;
        
        private void Start()
        {
            miniMapCamera = PlayerFind.instance.playerInstance.GetComponentInChildren<CameraCoordinator>()
                .miniMapCamera;
            initialPos = transform.position;
        }

        private void FixedUpdate()
        {
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            
            var screenPos = miniMapCamera.WorldToScreenPoint(initialPos);
            var cameraRect = miniMapCamera.pixelRect;
            screenPos.x = Mathf.Clamp(screenPos.x, cameraRect.xMin + offset.x, cameraRect.xMax - offset.x);
            screenPos.y = Mathf.Clamp(screenPos.y, cameraRect.yMin + offset.y, cameraRect.yMax - offset.y);
            transform.position = miniMapCamera.ScreenToWorldPoint(screenPos);
        }
    }
}