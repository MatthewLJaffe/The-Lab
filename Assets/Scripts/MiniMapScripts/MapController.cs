using System;
using UnityEngine;
using UnityEngine.Events;

namespace MiniMapScripts
{
    public class MapController : MonoBehaviour
    {
        public UnityEvent pullOutMap;
        public UnityEvent putAwayMap;

        private void Awake()
        {
            TeleporterInteractor.teleportInteract += ToggleTeleportMap;
        }

        private void OnDestroy()
        {
            TeleporterInteractor.teleportInteract -= ToggleTeleportMap;
        }

        public void PullOutMap()
        {
            pullOutMap.Invoke();
        }

        public void PutAwayMap()
        {
            putAwayMap.Invoke();
        }

        private void ToggleTeleportMap(bool enable)
        {
            if (enable)
                PullOutMap();
            else
                PutAwayMap();
        }
    }
}