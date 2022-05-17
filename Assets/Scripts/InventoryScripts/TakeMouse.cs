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
            PlayerInputManager.instance.DisableInput(PlayerInputManager.PlayerInputName.Fire1);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            PlayerInputManager.instance.EnableInput(PlayerInputManager.PlayerInputName.Fire1);
        }
    }
}