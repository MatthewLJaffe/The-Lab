using System;
using System.Linq;
using EntityStatsScripts;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace PlayerScripts
{
    public class PlayerInputManager : MonoBehaviour
    {
        public static Action<PlayerInputName> onInputDown = delegate {  };
        public static PlayerInputManager instance;
        private PlayerInput cancel;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                PlayerBar.BarDeplete += StopInput;
                PlayerWin.BroadcastWin += StopInput;
            }
            if (instance != this)
            {
                if (!instance.enabled)
                    instance.enabled = true;
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
            cancel = inputs.First(i => i.inputName == PlayerInputName.Cancel);
        }

        private void OnDestroy()
        {
            PlayerBar.BarDeplete -= StopInput;
            PlayerWin.BroadcastWin -= StopInput;
        }

        public enum PlayerInputName
        {
            Fire1,
            Interact,
            Inventory,
            Reload,
            Alpha_1,
            Alpha_2,
            Alpha_3,
            Alpha_4,
            Alpha_5,
            Alpha_6,
            Roll,
            Cancel,
            Map
        }
        [SerializeField] private PlayerInput[] inputs;


        private void StopInput()
        {
            enabled = false;
        }
        private void StopInput(PlayerBar.PlayerBarType bar)
        {
            enabled = false;
        }
        
        private void Update()
        {
            if (Time.timeScale == 0)
            {
                cancel.Pressed = Input.GetButton(cancel.inputName.ToString());
            }
            else
            {
                foreach (var input in inputs)
                    input.Pressed = Input.GetButton(input.inputName.ToString());
            }
        }

        public void DisableInput(PlayerInputName iName)
        {
            inputs.First(i => i.inputName == iName).usable = false;
        }
        
        public void EnableInput(PlayerInputName iName)
        {
            inputs.First(i => i.inputName == iName).usable = true;
        }

        public bool GetInput(PlayerInputName iName)
        {
            return inputs.Any(i => i.inputName == iName && i.Pressed && i.usable);
        }

        [Serializable]
        private class PlayerInput
        {
            public PlayerInputName inputName;
            public bool usable = true;
            private bool _pressed;
            public bool Pressed
            {
                get => _pressed;
                set
                {
                    if (value && value != _pressed && usable)
                        onInputDown.Invoke(inputName);
                    _pressed = value;
                }
            }
        }
    }
}
