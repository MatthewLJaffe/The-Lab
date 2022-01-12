using System;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace PlayerScripts
{
    public class PlayerInputManager : MonoBehaviour
    {
        public static Action<PlayerInputName> OnInputDown = delegate {  };
        public static PlayerInputManager instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            if (instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
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
            Alpha_6
        }
        [SerializeField] private PlayerInput[] inputs;
        
        
        private void Update()
        {
            foreach (var input in inputs) {
                input.Pressed = Input.GetButton(input.inputName.ToString());
            }
        }

        public bool GetInput(PlayerInputName iName)
        {
            return inputs.Any(i => i.inputName == iName && i.Pressed);
        }

        [Serializable]
        private class PlayerInput
        {
            public PlayerInputName inputName;
            private bool _pressed;
            public bool Pressed
            {
                get => _pressed;
                set
                {
                    if (value && value != _pressed)
                        OnInputDown.Invoke(inputName);
                    _pressed = value;
                }
            }
        }
    }
}
