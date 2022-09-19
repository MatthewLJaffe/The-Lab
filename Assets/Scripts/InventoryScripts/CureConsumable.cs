using PlayerScripts;
using UnityEngine;

namespace InventoryScripts
{
    public class CureConsumable : Consumable
    {
        private PlayerWin _win;
        protected override void Awake()
        {
            base.Awake();
            _win = PlayerFind.instance.playerInstance.GetComponent<PlayerWin>();
        }

        protected override void Consume(PlayerInputManager.PlayerInputName inputName)
        {
            if (inputName != PlayerInputManager.PlayerInputName.Fire1 || !gameObject.activeSelf) return;
            base.Consume(inputName);
            _win.WinGame();
        }
    }
}