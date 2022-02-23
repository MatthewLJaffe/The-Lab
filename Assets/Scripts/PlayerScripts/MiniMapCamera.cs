using System;
using UnityEngine;

namespace PlayerScripts
{
    [RequireComponent(typeof(Camera))]
    public class MiniMapCamera : MonoBehaviour
    {
        [SerializeField] private float zoomInSize;
        [SerializeField] private float zoomOutSize;
        [SerializeField] private RenderTexture miniMapRenderTexture;
        [SerializeField] private Vector2 minPos;
        [SerializeField] private Vector2 maxPos;
        [SerializeField] private float panSpeed;
        private bool _cameraMovementEnabled;
        private Camera _camera;
        private Camera _mainCamera;
        private Vector3 _moveVector;
        private Vector3 _initialPosition;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _mainCamera = Camera.main;
            _moveVector = Vector3.zero;
            _initialPosition = transform.position;
            ZoomIn();
        }

        public void ZoomIn()
        {
            transform.localPosition = _initialPosition;
            _mainCamera.gameObject.SetActive(true);
            _camera.orthographicSize = zoomInSize;
            _camera.targetTexture = miniMapRenderTexture;
            _cameraMovementEnabled = false;
        }

        public void ZoomOut()
        {
            _mainCamera.gameObject.SetActive(false);
            _camera.orthographicSize = zoomOutSize;
            _camera.targetTexture = null;
            _cameraMovementEnabled = true;
        }

        private void FixedUpdate()
        {
            if (!_cameraMovementEnabled) return;
            _moveVector.Set
                (Input.GetAxis("Horizontal") * panSpeed * Time.fixedDeltaTime, Input.GetAxis("Vertical") * panSpeed * Time.fixedDeltaTime, 0);
            var position = transform.position;
            var newPos = position + _moveVector;
            newPos.x = Mathf.Clamp(newPos.x, minPos.x, maxPos.x);
            newPos.y = Mathf.Clamp(newPos.y, minPos.y, maxPos.y);
            position = newPos;
            transform.position = position;
        }
    }
}