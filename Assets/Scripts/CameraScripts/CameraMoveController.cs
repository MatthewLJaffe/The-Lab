using System;
using PlayerScripts;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CameraScripts
{
    public class CameraMoveController : MonoBehaviour
    {
        private Transform _playerTrans;
        [SerializeField] private float maxDistanceXFromPlayer;
        [SerializeField] private float maxDistanceYFromPlayer;
        private Vector2 _mousePos;
        private Vector2 _targetPos;

        private void Start()
        {
            _playerTrans = PlayerFind.instance.playerInstance.transform;
        }

        private void LateUpdate()
        {
            _mousePos = Input.mousePosition;
            var normX = Mathf.Clamp(_mousePos.x / Screen.width - .5f, -1f, 1f);
            var normY = Mathf.Clamp(_mousePos.y / Screen.height - .5f, -1f, 1f);
            _targetPos.x = normX * maxDistanceXFromPlayer;
            _targetPos.y = normY * maxDistanceYFromPlayer;
            transform.localPosition = _targetPos;
        }
    }
}