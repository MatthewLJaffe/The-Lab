using System;
using PlayerScripts;
using UnityEngine;
using UnityEngine.Events;

namespace MiniMapScripts
{
    public class MapController : MonoBehaviour
    {
        public UnityEvent pullOutMap;
        public UnityEvent putAwayMap;
        private bool _enabled;

        private void Awake()
        {
            TeleporterInteractor.teleportInteract += ToggleTeleportMap;
        }

        private void OnDestroy()
        {
            TeleporterInteractor.teleportInteract -= ToggleTeleportMap;
        }

        private void Start()
        {
            PlayerInputManager.onInputDown += TryToggleMap;
        }

        private void TryToggleMap(PlayerInputManager.PlayerInputName iName)
        {
            if (iName != PlayerInputManager.PlayerInputName.Map) return;
            ToggleTeleportMap(!_enabled);
            
        }

        public void PullOutMap()
        {
            _enabled = true;
            pullOutMap.Invoke();
        }

        public void PutAwayMap()
        {
            _enabled = false;
            putAwayMap.Invoke();
        }

        private void ToggleTeleportMap(bool enable)
        {
            _enabled = enable;
            if (enable)
                PullOutMap();
            else
                PutAwayMap();
        }
    }
}