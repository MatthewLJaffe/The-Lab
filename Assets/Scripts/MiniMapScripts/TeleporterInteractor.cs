using PlayerScripts;
using UnityEngine;

namespace MiniMapScripts
{
    public class TeleporterInteractor : MonoBehaviour, IInteractable
    {
        public bool CanInteract { get; set; }
        public void Interact()
        {
            throw new System.NotImplementedException();
        }
    }
}