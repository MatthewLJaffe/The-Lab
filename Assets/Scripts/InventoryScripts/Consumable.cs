using System;
using PlayerScripts;
using UnityEngine;
using UnityEngine.Events;

namespace InventoryScripts
{
    public abstract class Consumable : MonoBehaviour
    {
        public UnityEvent onConsume;
        public Action ItemConsumed = delegate {  };
        
        protected virtual void Awake()
        {
            PlayerInputManager.OnInputDown += Consume;
        }

        protected virtual void OnDestroy()
        {
            PlayerInputManager.OnInputDown -= Consume;
        }

        protected virtual void Consume(PlayerInputManager.PlayerInputName inputName)
        {
            if (inputName != PlayerInputManager.PlayerInputName.Fire1 || !gameObject.activeSelf) return;
            ItemConsumed.Invoke();
            onConsume.Invoke();
        }
    }
}