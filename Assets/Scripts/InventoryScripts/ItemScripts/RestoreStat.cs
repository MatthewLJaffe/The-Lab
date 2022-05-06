using System.Linq;
using EntityStatsScripts;
using General;
using PlayerScripts;
using TMPro;
using UnityEngine;

namespace InventoryScripts.ItemScripts
{
    public class RestoreStat : RestoreConsumable
    {
        [SerializeField] private float restoreAmount;
        [SerializeField] private GameObject restoreTextPrefab;
        private GameObjectPool _textPool;
        private Transform _displayPoint;


        protected override void Awake()
        {
            base.Awake();
            var worldSpaceCanvas = transform.root.GetComponentsInChildren<Canvas>()
                .First(c => c.renderMode == RenderMode.WorldSpace);
            _displayPoint = worldSpaceCanvas.transform.GetChild(0);
            _textPool = new GameObjectPool(restoreTextPrefab, _displayPoint);
        }
        
        protected override void Consume(PlayerInputManager.PlayerInputName inputName)
        {
            if (inputName != PlayerInputManager.PlayerInputName.Fire1 || !gameObject.activeSelf) return;
            var finalRestoreAmount = Mathf.Round(restoreAmount * restoreMultiplier);
            PlayerBarsManager.Instance.ModifyPlayerStat(restoreType, finalRestoreAmount);
            var restoreText = _textPool.GetFromPool();
            restoreText.GetComponent<TextMeshProUGUI>().text = "+" + finalRestoreAmount;
            base.Consume(inputName);
        }
        
        
    }
}