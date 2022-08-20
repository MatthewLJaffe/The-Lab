using System;
using TMPro;
using UnityEngine;

namespace EntityStatsScripts
{
    public class AtkDefDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI atkText;
        [SerializeField] private TextMeshProUGUI defText;

        private void Awake()
        {
            PlayerStats.onStatChange += UpdateStats;
        }

        private void OnDestroy()
        {
            PlayerStats.onStatChange -= UpdateStats;
        }

        private void UpdateStats(PlayerStats.StatType type, float newVal)
        {
            if (type == PlayerStats.StatType.Attack)
                atkText.text = $"{newVal}";
            if (type == PlayerStats.StatType.Defense)
                defText.text = $"{newVal}";
        }
    }
}