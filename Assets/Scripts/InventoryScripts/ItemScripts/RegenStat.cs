using System.Collections;
using System.Linq;
using EntityStatsScripts;
using General;
using TMPro;
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
        private Coroutine _regenCoroutine;

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
            _regenCoroutine = StartCoroutine(Regen(type, amount, duration, textPrefab));
        }

        public void StopRegen()
        {
            if (_regenCoroutine != null)
            {
                StopCoroutine(_regenCoroutine);
                _regenCoroutine = null;
            }
        }

        public void StartIndefiniteRegen(PlayerBar.PlayerBarType type, float amountPerTick, float tickTime)
        {
            _regenCoroutine = StartCoroutine(IndefiniteRegen(type, amountPerTick, tickTime));
        }
        private IEnumerator Regen(PlayerBar.PlayerBarType type, float amount, float duration, GameObject textPrefab)
        {
            if (textPrefab)
                _textPool = new GameObjectPool(textPrefab, _displayPoint);
            var restorePerSec = amount / duration;
            float amountRestored = 0;
            var wait = new WaitForSeconds(1);
            for (; amountRestored < amount; amountRestored += restorePerSec)
            {
                PlayerBarsManager.Instance.ModifyPlayerStat(type, restorePerSec);
                if (_textPool != null) {
                    var restoreText = _textPool.GetFromPool();
                    restoreText.GetComponent<TextMeshProUGUI>().text = "+" + Mathf.Round(restorePerSec);
                }
                yield return wait;
            }
            Destroy(this);
        }

        private IEnumerator IndefiniteRegen(PlayerBar.PlayerBarType type, float amountPerTick, float tickTime)
        {
            var wait = new WaitForSeconds(amountPerTick);
            while (true)
            {
                PlayerBarsManager.Instance.ModifyPlayerStat(type, amountPerTick);
                yield return wait;
            }
        }
    }
}