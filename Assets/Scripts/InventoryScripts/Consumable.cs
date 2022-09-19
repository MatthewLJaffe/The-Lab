using System;
using PlayerScripts;
using UnityEngine;
using UnityEngine.Events;

namespace InventoryScripts
{
    public abstract class Consumable : MonoBehaviour
    {
        public UnityEvent onUse;
        public Action itemConsumed = delegate {  };
        
        protected virtual void Awake()
        {
            PlayerInputManager.onInputDown += Consume;
        }

        protected virtual void OnDestroy()
        {
            PlayerInputManager.onInputDown -= Consume;
        }

        protected virtual void Consume(PlayerInputManager.PlayerInputName inputName)
        {
            if (inputName != PlayerInputManager.PlayerInputName.Fire1 || !gameObject.activeSelf) return;
            onUse.Invoke();
            itemConsumed.Invoke();
        }
    }
}