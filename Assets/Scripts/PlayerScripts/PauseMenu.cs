using System;
using UnityEngine;

namespace PlayerScripts
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject UIParent;
        private bool _paused;
        private void Awake()
        {
            PlayerInputManager.onInputDown += PressPause;
        }

        private void OnDestroy()
        {
            PlayerInputManager.onInputDown -= PressPause;
        }

        private void PressPause(PlayerInputManager.PlayerInputName iName)
        {
            if (iName != PlayerInputManager.PlayerInputName.Cancel) return;
            TogglePause();
        }

        public void TogglePause()
        {
            _paused = !_paused;
            UIParent.SetActive(_paused);
            if (_paused)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;
        }
        
    }
}