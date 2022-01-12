using System;
using System.Collections.Generic;
using UnityEngine;

namespace EntityStatsScripts
{
    [CreateAssetMenu(fileName = "PlayerStats", menuName = "")]
    public class PlayerStats : ScriptableObject
    {
        public static Action<StatType, float> OnStatChange = delegate { };
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
            FireRate
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
                    OnStatChange.Invoke(type, value);
                }
            }
            public void Initialize()
            {
                CurrentValue = defaultValue;
            }
        }
        [SerializeField] private PlayerStat[] stats;
        public Dictionary<StatType, PlayerStat> PlayerStatsDict;

        public void Awake()
        {
            PlayerStatsDict = new Dictionary<StatType, PlayerStat>();
            foreach (var stat in stats) {
                stat.Initialize();
                PlayerStatsDict.Add(stat.type, stat);
            }
        }
    }
}