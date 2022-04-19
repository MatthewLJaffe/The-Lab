using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerScripts
{
    public class PlayerInteractor : MonoBehaviour
    {
        private Dictionary<GameObject, IInteractable> _interactables;

        private void Awake()
        {
            _interactables = new Dictionary<GameObject, IInteractable>();
            PlayerInputManager.onInputDown += InteractWithObjects;
        }

        private void OnDestroy()
        {
            PlayerInputManager.onInputDown -= InteractWithObjects;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var interact = other.gameObject.GetComponent<IInteractable>();
            if (interact == null) return;
            interact.CanInteract = true;
            _interactables.Add(other.gameObject, interact);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var otherGameObject = other.gameObject;
            if (!_interactables.ContainsKey(otherGameObject)) return;
            var interact = _interactables[other.gameObject];
            interact.CanInteract = false;
            _interactables.Remove(otherGameObject);
        }
        

        private void InteractWithObjects(PlayerInputManager.PlayerInputName inputName)
        {
            if (inputName != PlayerInputManager.PlayerInputName.Interact) return;
            foreach (var i in _interactables.Values) {
                i.Interact();
            }
        }
    }
}