using System.Collections.Generic;
using General;
using TMPro;
using UnityEngine;

namespace EntityStatsScripts
{
    /// <summary>
    /// used to update player health / infection stats and bars
    /// </summary>
    public class PlayerBarsManager : MonoBehaviour
    {
        public static PlayerBarsManager Instance;
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private SoundEffect healSound;
        [SerializeField] private SoundEffect damageSound;
        [SerializeField] private GameObject numberPrefab;
        [SerializeField] private Transform displayPoint;
        [SerializeField] private Color healthColor;
        [SerializeField] private Color infectionColor;
        private float _speedEffect;
        private float _defenceEffect;
        private float _strengthEffect;
        private float _critChanceEffect;
        private GameObjectPool _barNumberPool;
        private Dictionary<PlayerBar.PlayerBarType, PlayerBar> _playerBars;

        private void Awake()
        {
            if (Instance != this)
            {
                if (Instance)
                    Destroy(Instance.gameObject);
                Instance = this;
            }
            _barNumberPool = new GameObjectPool(numberPrefab, displayPoint);

        }

        private void Start()
        {
            InitializePlayerStats();
        }

        private void InitializePlayerStats()
        {
            playerStats.Awake();
            _playerBars = new Dictionary<PlayerBar.PlayerBarType, PlayerBar>();
            foreach (var playerStat in GetComponentsInChildren<PlayerBar>()) {
                _playerBars.Add(playerStat.barType, playerStat);
                playerStat.Initialize();
            }
        }

        public void ModifyAndDisplayStat(PlayerBar.PlayerBarType type, float amount)
        {
            ModifyPlayerStat(type, amount);
            var numberTmp = _barNumberPool.GetFromPool().GetComponent<TextMeshProUGUI>();
            numberTmp.text = $"{Mathf.RoundToInt(amount)}";
            switch (type)
            {
                case PlayerBar.PlayerBarType.Health:
                    numberTmp.color = healthColor;
                    if (amount > 0)
                    {
                        numberTmp.text = "+" + numberTmp.text;
                        healSound.Play();
                    }
                    else if (amount < 0)
                    {
                        damageSound.Play();
                    }
                    break;
                case PlayerBar.PlayerBarType.Infection:
                    numberTmp.color = infectionColor;
                    break;
            }
        }

        public void ModifyPlayerStat(PlayerBar.PlayerBarType type, float amount)
        {
            _playerBars[type].BarValue += amount;
        }
    }
}