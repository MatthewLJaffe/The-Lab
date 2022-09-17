using UnityEngine;

namespace MiniMapScripts
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
        [SerializeField] private float mousePanSpeed;
        private bool _cameraMovementEnabled;
        private Camera _camera;
        private Vector3 _moveVector;
        private Vector3 _initialPosition;
        private Vector3 _prevMousePos;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _moveVector = Vector3.zero;
            _initialPosition = transform.position;
            ZoomIn();
            _prevMousePos = Vector3.zero;
        }

        public void ZoomIn()
        {
            transform.localPosition = _initialPosition;
            _camera.orthographicSize = zoomInSize;
            _camera.targetTexture = miniMapRenderTexture;
            _cameraMovementEnabled = false;
        }

        public void ZoomOut()
        {
            _camera.orthographicSize = zoomOutSize;
            _camera.targetTexture = null;
            _cameraMovementEnabled = true;
        }

        private void FixedUpdate()
        {
            if (_cameraMovementEnabled)
                HandleKeyboardPan();
        }

        private void Update()
        {
            if (_cameraMovementEnabled)
                HandleMousePan();
        }

        private void HandleKeyboardPan()
        {
            _moveVector.Set(Input.GetAxis("Horizontal") * panSpeed * Time.fixedDeltaTime, Input.GetAxis("Vertical") * panSpeed * Time.fixedDeltaTime, 0);
            var position = transform.position;
            var newPos = position + _moveVector;
            newPos.x = Mathf.Clamp(newPos.x, minPos.x, maxPos.x);
            newPos.y = Mathf.Clamp(newPos.y, minPos.y, maxPos.y);
            position = newPos;
            transform.position = position;
        }

        private void HandleMousePan()
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                _prevMousePos =  Camera.main.ScreenToWorldPoint(Input.mousePosition);
                return;
            }
            
            if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)) return;
            var currMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var moveDir = _prevMousePos - currMousePos;
            if (moveDir.magnitude > .001f && moveDir.magnitude < 5f)
            {
                _moveVector.Set(moveDir.x * mousePanSpeed ,moveDir.y * mousePanSpeed , 0);
                var position = transform.position;
                var newPos = position + _moveVector;
                newPos.x = Mathf.Clamp(newPos.x, minPos.x, maxPos.x);
                newPos.y = Mathf.Clamp(newPos.y, minPos.y, maxPos.y);
                position = newPos;
                transform.position = position;
            }
            _prevMousePos = currMousePos;
        }
    }
}