using InventoryScripts;
using PlayerScripts;
using UnityEngine;

namespace EntityStatsScripts.Effects
{
    public class ApplyEffect : Consumable
    {
        [SerializeField] private Effect effect;

        protected override void Consume(PlayerInputManager.PlayerInputName inputName)
        {
            if (inputName != PlayerInputManager.PlayerInputName.Fire1 || !gameObject.activeSelf) return;
            if (effect)
                effect.Stack++;
            base.Consume(inputName);
        }
    }
}