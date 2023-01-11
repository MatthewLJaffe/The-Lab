using UnityEngine;

namespace CameraScripts
{
    /// <summary>
    /// Groups the different cameras used in the player prefab
    /// </summary>
    public class CameraCoordinator : MonoBehaviour
    {
        [SerializeField] private Camera main;
        [SerializeField] private Camera background;
        public Camera miniMapCamera;
        private float _currentSize;

        private void Start() {
            _currentSize = main.orthographicSize;
            background.orthographicSize = _currentSize;
        }
    }
}
