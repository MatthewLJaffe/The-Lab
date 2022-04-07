using System;
using EntityStatsScripts;
using PlayerScripts;
using UnityEngine;

namespace EnemyScripts
{
    public class Enemy : MonoBehaviour
    {
        public Action enemyKilled = delegate {  };
        public BoxCollider2D spawnCollider;
        public Transform target;
        [SerializeField] private bool enteredRoom;
        [SerializeField] private Transform roomChild;
        [SerializeField] private SpriteRenderer sr;
        private Animator _animator;
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
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
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

        public void Death()
        {
            var sprite = sr.sprite;
            if (_animator)
                _animator.enabled = false;
            sr.sprite = sprite;
            enemyKilled.Invoke();
            var colliders = GetComponents<Collider2D>();
            if (colliders == null) return;
            foreach (var c in colliders)
                c.enabled = false;
            
        }
    }
}