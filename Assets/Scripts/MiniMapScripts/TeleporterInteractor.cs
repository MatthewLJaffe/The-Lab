using System;
using PlayerScripts;
using UnityEngine;

namespace MiniMapScripts
{
    public class TeleporterInteractor : MonoBehaviour, IInteractable
    {
        public static Action<bool> teleportInteract = delegate { };
        private static bool _teleportEnabled;
        [SerializeField] private Color interactColor;
        [SerializeField] private SpriteRenderer sr;

        public bool CanInteract
        {
            set => sr.color = value ? interactColor : Color.white;
        }

        public void Interact()
        {
            _teleportEnabled = !_teleportEnabled;
            teleportInteract.Invoke(_teleportEnabled);
        }
    }
}