using System;
using PlayerScripts;
using UnityEngine;
using UnityEngine.Events;

namespace InventoryScripts
{
    public class DetectSelection : MonoBehaviour
    {
        private bool _enter;
        private RectTransform _rectTransform;
        public UnityEvent clickInsideArea;
        
        private void Awake()
        {
            PlayerInputManager.onInputDown += CheckForPress;
            _rectTransform = transform as RectTransform;
        }

        private void OnDestroy()
        {
            PlayerInputManager.onInputDown -= CheckForPress;
        }

        private void CheckForPress(PlayerInputManager.PlayerInputName inputName)
        {
            if (inputName == PlayerInputManager.PlayerInputName.Fire1 &&
                Mathf.Abs((Input.mousePosition - transform.position).x) < _rectTransform.sizeDelta.x / 2 &&
                Mathf.Abs((Input.mousePosition - transform.position).y) < _rectTransform.sizeDelta.y / 2) {
                clickInsideArea.Invoke();
            }
        }
    }
}