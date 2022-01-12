using System;
using System.Collections;
using System.Linq;
using EntityStatsScripts;
using General;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace InventoryScripts.ItemScripts
{
    public class RegenStat : MonoBehaviour
    {
        public PlayerBar.PlayerBarType restoreType;
        [SerializeField] private float restoreAmount;
        [SerializeField] private float restoreDuration;
        [SerializeField] private GameObject restoreTextPrefab;
        [SerializeField] private bool startAutomatically;
        private GameObjectPool _textPool;
        private Transform _displayPoint;

        private void Awake()
        {
            var worldSpaceCanvas = transform.root.GetComponentsInChildren<Canvas>()
                .First(c => c.renderMode == RenderMode.WorldSpace);
            _displayPoint = worldSpaceCanvas.transform.GetChild(0);
            _textPool = new GameObjectPool(restoreTextPrefab, _displayPoint);
            
        }

        private void Start()
        {
            if (startAutomatically)
                StartCoroutine(Regen(restoreType, restoreAmount, restoreDuration, restoreTextPrefab));
        }

        public void StartRegen(PlayerBar.PlayerBarType type, float amount, float duration, GameObject textPrefab)
        {
            StartCoroutine(Regen(type, amount, duration, textPrefab));
        }
        private IEnumerator Regen(PlayerBar.PlayerBarType type, float amount, float duration, GameObject textPrefab)
        {
            _textPool = new GameObjectPool(textPrefab, _displayPoint);
            var restorePerSec = amount / duration;
            float amountRestored = 0;
            for (; amountRestored < amount; amountRestored += restorePerSec)
            {
                PlayerBarsManager.Instance.ModifyPlayerStat(type, restorePerSec);
                var restoreText = _textPool.GetFromPool();
                restoreText.GetComponent<TextMeshProUGUI>().text = "+" + restorePerSec;
                yield return new WaitForSeconds(1);
            }
            Destroy(this);
        }
    }
}