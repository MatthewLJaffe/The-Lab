using System.Collections.Generic;
using UnityEngine;

namespace EntityStatsScripts
{
    public class PlayerBarsManager : MonoBehaviour
    {
        public static PlayerBarsManager Instance;
        [SerializeField] private PlayerStats playerStats;
        private float _speedEffect;
        private float _defenceEffect;
        private float _strengthEffect;
        private float _critChanceEffect;
        private Dictionary<PlayerBar.PlayerBarType, PlayerBar> _playerBars;

        private void Awake()
        {
            if (Instance != this)
            {
                if (Instance)
                    Destroy(Instance.gameObject);
                Instance = this;
            }
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

        public void ModifyPlayerStat(PlayerBar.PlayerBarType type, float amount)
        {
            _playerBars[type].BarValue += amount;
        }
    }
}