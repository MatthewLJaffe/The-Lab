using PlayerScripts;
using UnityEngine;

namespace MiniMapScripts
{
    public class AssignMiniMapCamera : MonoBehaviour
    {
        private Camera _miniMapCamera;
        private Canvas _canvas;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
        }

        private void Start()
        {
            _miniMapCamera = PlayerFind.instance.playerInstance.GetComponentInChildren<CameraCoordinator>().miniMapCamera;
            _canvas.worldCamera = _miniMapCamera;
        }
    }
}