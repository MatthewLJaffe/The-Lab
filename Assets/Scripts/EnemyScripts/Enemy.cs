using System;
using PlayerScripts;
using UnityEngine;

namespace EnemyScripts
{
    public class Enemy : MonoBehaviour
    {
        public BoxCollider2D spawnCollider;
        public Transform target;
        [SerializeField] private bool enteredRoom;
        [SerializeField] private Transform roomChild;
        public bool EnteredRoom
        {
            get => enteredRoom;
            set => enteredRoom = value;
        }
        [SerializeField] private Vector3 spawnPos;

        public Vector3 SpawnPos
        {
            get => spawnPos;
            set
            {
                spawnPos = value;
                if (!gameObject.activeInHierarchy)
                    transform.position = spawnPos;
            }
        }
        
        public Action enemyKilled = delegate {  };

        private void Awake()
        {
            spawnPos = transform.position;
            PlayerFind.onPlayerReset += go => target = go.transform;
            if (roomChild && roomChild.parent)
            {
                var enemyHandler = roomChild.parent.GetComponentInChildren<EnemyHandler>();
                if (enemyHandler && !enemyHandler.enemies.Contains(this)) {
                    enemyHandler.enemies.Add(this);
                    enemyKilled += enemyHandler.IncrementDead;
                }
            }
        }

        private void Start()
        {
            if (PlayerFind.instance.playerInstance)
                target = PlayerFind.instance.playerInstance.transform;
            else
                target = null;
        }

        public void OnDestroy()
        {
            enemyKilled.Invoke();
        }
    }
}