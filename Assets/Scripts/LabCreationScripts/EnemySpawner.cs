using System;
using EnemyScripts;
using LabCreationScripts.ProceduralRooms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LabCreationScripts
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Transform roomChild;
        [SerializeField] private EnemyWeight[] enemyWeights;
        private void Start()
        {
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