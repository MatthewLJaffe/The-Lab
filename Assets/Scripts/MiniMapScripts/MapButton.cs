using CameraScripts;
using MiniMapScripts;
using PlayerScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace General
{
    public class MapButton : MonoBehaviour
    {
        private Camera _mapCamera;
        [SerializeField] private Image image;
        public UnityEvent onMapButtonPress;
        
        private bool _pressable;
        
        

        private void Awake()
        {
            TeleporterInteractor.teleportInteract += SetPressable;
        }

        private void OnDestroy()
        {
            TeleporterInteractor.teleportInteract -= SetPressable;
            PlayerInputManager.onInputDown -= CheckButtonPress;
        }
        
        private void Start()
        {
            _mapCamera =  PlayerFind.instance.playerInstance.GetComponentInChildren<CameraCoordinator>().miniMapCamera;
            PlayerInputManager.onInputDown += CheckButtonPress;
        }

        private void SetPressable(bool canPress)
        {
            _pressable = canPress;
        }

        private void CheckButtonPress(PlayerInputManager.PlayerInputName iName)
        {
            if (iName != PlayerInputManager.PlayerInputName.Fire1 || !_pressable) return;
            var mousePos = _mapCamera.ScreenToWorldPoint(Input.mousePosition);
            if (InsideButton(mousePos))
                onMapButtonPress.Invoke();
        }

        private bool InsideButton(Vector2 mouseWorldPos)
        {
            var rectTransform = image.rectTransform;
            return mouseWorldPos.x < rectTransform.position.x + rectTransform.rect.width / 2 &&
                   mouseWorldPos.x > rectTransform.position.x - rectTransform.rect.width / 2 &&
                   mouseWorldPos.y < rectTransform.position.y + rectTransform.rect.height / 2 &&
                   mouseWorldPos.y > rectTransform.position.y - rectTransform.rect.height / 2;
        }
    }
}