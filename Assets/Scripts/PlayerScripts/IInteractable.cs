using UnityEngine;

namespace PlayerScripts
{
    public interface IInteractable
    {
        bool CanInteract { set; }
        void Interact();
    }
}