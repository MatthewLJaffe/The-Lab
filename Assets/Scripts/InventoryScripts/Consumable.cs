using System;
using PlayerScripts;
using UnityEngine;

namespace InventoryScripts
{
    public abstract class Consumable : MonoBehaviour
    {
        
        public Action ItemConsumed = delegate {  };
        
        protected virtual void Awake()
        {
            PlayerInputManager.OnInputDown += Consume;
        }

        protected void OnDestroy()
        {
            PlayerInputManager.OnInputDown -= Consume;
        }

        protected virtual void Consume(PlayerInputManager.PlayerInputName inputName)
        {
            if (inputName != PlayerInputManager.PlayerInputName.Fire1 || !gameObject.activeSelf) return;
            ItemConsumed.Invoke();
        }
    }
}