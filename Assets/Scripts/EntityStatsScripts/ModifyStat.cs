using System;
using System.Linq;
using General;
using UnityEngine;

namespace EntityStatsScripts
{
    public class ModifyStat : MonoBehaviour
    {
        [SerializeField] private PlayerBar.PlayerBarType statType;

        public void ChangeStat(float amount)
        {
            PlayerBarsManager.Instance.ModifyPlayerStat(statType, amount);
        }
    }
}