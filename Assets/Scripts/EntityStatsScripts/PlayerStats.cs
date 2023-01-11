using System;
using System.Collections.Generic;
using UnityEngine;

namespace EntityStatsScripts
{
    /// <summary>
    /// scriptable object that contains state of all player stats
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerStats", menuName = "")]
    public class PlayerStats : ScriptableObject
    {
        public static Action<StatType, float> onStatChange = delegate { };
        public enum StatType
        {
            MaxHealth,
            MaxDisease,
            Speed,
            Attack,
            CritChance,
            Defense,
            Accuracy,
            DodgeChance,
            FireRate,
            ReloadFactor,
            RestoreMultiplier,
            DontConsumeChance,
            RegenPerTick,
            CritMultiplier,
            CurrentFloor
        }
        
        [Serializable]
        public class PlayerStat
        {
            public StatType type;
            [SerializeField] private float currentValue;
            public float defaultValue;
            public float CurrentValue
            {
                get => currentValue;
                set
                {
                    currentValue = value;
                    onStatChange.Invoke(type, value);
                }
            }
            public void Initialize()
            {
                CurrentValue = defaultValue;
            }
        }

        [SerializeField] private float atkToDamageMultiplier;
        [SerializeField] private PlayerStat[] stats;
        public Dictionary<StatType, PlayerStat> playerStatsDict;

        public void Awake()
        {
            playerStatsDict = new Dictionary<StatType, PlayerStat>();
            foreach (var stat in stats) {
                playerStatsDict.Add(stat.type, stat);
                stat.Initialize();
            }
        }

        public float GetAttackMultiplier()
        {
            return 1f + atkToDamageMultiplier * playerStatsDict[StatType.Attack].CurrentValue;
        }

        public void IncrementAttack()
        {
            playerStatsDict[StatType.Attack].CurrentValue++;
        }

        public void IncrementDef()
        {
            playerStatsDict[StatType.Defense].CurrentValue++;
        }
        
    }
}