using System;
using System.Collections.Generic;
using System.Linq;
using LabCreationScripts;
using UnityEngine;

namespace EnemyScripts
{
    public class EnemyHandler : MonoBehaviour
    {
        public static Action<Room> onRoomClear = delegate {  };
        public List<Enemy> enemies;

        public Room myRoom;
        private int _enemiesDead;
        private bool _areEnemiesDefeated;

        private void Awake()
        {
            Door.onEnterRoom += TrySpawnEnemies;
            
        }

        private void Start()
        {
            var enemyArr = transform.parent.GetComponentsInChildren<Enemy>();
            if (enemyArr != null)
            {
                enemies = enemyArr.ToList();
                foreach (var enemy in enemies) {
                    enemy.gameObject.SetActive(false);
                }
            }
            else
                enemies = new List<Enemy>();
        }

        private void OnDestroy()
        {
            Door.onEnterRoom -= TrySpawnEnemies;
            if (enemies != null)
            {
                foreach (var enemy in enemies) {
                    enemy.enemyKilled -= IncrementDead;
                }
            }
        }

        public void IncrementDead()
        {
            _enemiesDead++;
            if (_enemiesDead < enemies.Count) return;
            _areEnemiesDefeated = true;
            enemies.Clear();
            onRoomClear.Invoke(myRoom);
        }

        private void TrySpawnEnemies(Room room)
        {
            if (_areEnemiesDefeated) return;
            //if player has entered the enemies room they will already be active and need to know the player is in their room
            if (room.RoomId == myRoom.RoomId)
            {
                foreach (var enemy in enemies) {
                    enemy.gameObject.SetActive(true);
                }
            }
            else 
            {
                foreach (var enemy in enemies)
                    enemy.gameObject.SetActive(false);
            }
        }
    }
}