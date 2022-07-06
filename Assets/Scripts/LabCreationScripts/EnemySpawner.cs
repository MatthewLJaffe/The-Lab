using System;
using EnemyScripts;
using EntityStatsScripts;
using LabCreationScripts.ProceduralRooms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LabCreationScripts
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private Transform roomChild;
        [SerializeField] private EnemyWeight[] enemyWeightsFloor1;
        [SerializeField] private EnemyWeight[] enemyWeightsFloor2;
        [SerializeField] private EnemyWeight[] enemyWeightsFloor3;


        private void Start()
        {
            var floor = Mathf.RoundToInt(playerStats.playerStatsDict[PlayerStats.StatType.CurrentFloor].CurrentValue);
            var enemyWeights = enemyWeightsFloor1;
            switch (floor)
            {
                case 1:
                    break;
                case 2:
                    enemyWeights = enemyWeightsFloor2;
                    break;
                case 3:
                    enemyWeights = enemyWeightsFloor3;
                    break;
            }
            var value = Random.Range(0f, 1f);
            foreach (var ew in enemyWeights)
            {
                if (ew.weight >= value) {
                    Instantiate(ew.enemy, transform.position, Quaternion.identity, transform.parent)
                        .GetComponent<Enemy>().roomChild = roomChild;
                    return;                
                }
                value -= ew.weight;
            }
        }

        [Serializable]
        private struct EnemyWeight
        {
            public float weight;
            public GameObject enemy;
        }
    }
}