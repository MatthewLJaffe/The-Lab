using System;
using PlayerScripts;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InventoryScripts
{
    public class TakeMouse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("Mouse Entered " + gameObject.name);
            PlayerInputManager.instance.DisableInput(PlayerInputManager.PlayerInputName.Fire1);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("Mouse Exited " + gameObject.name);
            PlayerInputManager.instance.EnableInput(PlayerInputManager.PlayerInputName.Fire1);
        }
    }
}