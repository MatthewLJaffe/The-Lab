using System;
using System.Linq;
using General;
using UnityEngine;

namespace EntityStatsScripts
{
    /// <summary>
    /// component used to change player health and infection status
    /// </summary>
    public class ModifyStat : MonoBehaviour
    {
        [SerializeField] private float changeAmount;
        [SerializeField] private PlayerBar.PlayerBarType statType;

        public void ChangeStat(float amount)
        {
            PlayerBarsManager.Instance.ModifyAndDisplayStat(statType, amount);
        }

        public void ChangeStat()
        {
            PlayerBarsManager.Instance.ModifyAndDisplayStat(statType, changeAmount);
        }
    }
}