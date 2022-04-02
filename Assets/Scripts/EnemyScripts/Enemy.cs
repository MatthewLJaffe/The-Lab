﻿using System;
using EntityStatsScripts;
using PlayerScripts;
using UnityEngine;

namespace EnemyScripts
{
    public class Enemy : MonoBehaviour
    {
        public Action EnemyKilled = delegate {  };
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

        public void Death()
        {
            var flash = GetComponentInChildren<TakeDamageFlash>();
            if (flash) {
                flash.DeathFlash();
            }
            var steer = GetComponent<SteeringController>();
            if (steer)
                steer.speed = 0;
            var colliders = GetComponents<Collider2D>();
            if (colliders != null)
            {
                foreach (var c in colliders)
                    c.enabled = false;
            }
        }

        private void Awake()
        {
            spawnPos = transform.position;
            PlayerFind.onPlayerReset += go => target = go.transform;
            if (roomChild && roomChild.parent)
            {
                var enemyHandler = roomChild.parent.GetComponentInChildren<EnemyHandler>();
                if (enemyHandler && !enemyHandler.enemies.Contains(this)) {
                    enemyHandler.enemies.Add(this);
                    EnemyKilled += enemyHandler.IncrementDead;
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
            EnemyKilled.Invoke();
        }
    }
}