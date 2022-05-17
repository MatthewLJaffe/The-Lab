using System;
using System.Collections;
using EntityStatsScripts;
using PlayerScripts;
using UnityEngine;
using UnityEngine.Events;

namespace EnemyScripts
{
    public class Enemy : MonoBehaviour
    {
        public Action enemyKilled = delegate {  };
        public BoxCollider2D spawnCollider;
        public Transform target;
        public Transform roomChild;
        [SerializeField] private SpriteRenderer sr;
        private Animator _animator;
        [SerializeField] private float spawnInTime;
        public UnityEvent<float> spawnInBegin;
        public UnityEvent spawnInComplete;
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
        }

        private void OnEnable()
        {
            StartCoroutine(SpawnIn());
        }

        private IEnumerator SpawnIn()
        {
            spawnInBegin.Invoke(spawnInTime);
            _animator.Play("SpawnIn");
            yield return new WaitForSeconds(spawnInTime);
            spawnInComplete.Invoke();
        }

        private void Start()
        {
            if (PlayerFind.instance.playerInstance)
                target = PlayerFind.instance.playerInstance.transform;
            else
                target = null;
            if (roomChild && roomChild.parent)
            {
                var enemyHandler = roomChild.parent.GetComponentInChildren<EnemyHandler>();
                if (enemyHandler && !enemyHandler.enemies.Contains(this)) {
                    enemyHandler.enemies.Add(this);
                    //enemyKilled += enemyHandler.IncrementDead;
                    gameObject.SetActive(false);
                }
            }
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