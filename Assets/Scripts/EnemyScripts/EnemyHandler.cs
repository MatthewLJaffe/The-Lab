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
        private static bool _firstRoomEnemiesSpawned;

        private void Awake()
        {
            Door.onEnterRoom += TrySpawnEnemies;
            if (enemies == null)
                enemies = new List<Enemy>();
            foreach (var enemy in transform.parent.GetComponentsInChildren<Enemy>(true))
            {
                if (enemies.Contains(enemy)) continue;
                enemies.Add(enemy);
                enemy.EnemyKilled += IncrementDead;
            }
        }

        private void OnDestroy()
        {
            Door.onEnterRoom -= TrySpawnEnemies;
            if (enemies != null)
            {
                foreach (var enemy in enemies) {
                    enemy.EnemyKilled -= IncrementDead;
                }
            }
        }
        
        private void Start()
        {
            if (_firstRoomEnemiesSpawned) return;
            _firstRoomEnemiesSpawned = true;
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
                foreach (var enemy in enemies)
                    enemy.EnteredRoom = true;
                return;
            }
            //otherwise check if the player is in a neighboring room and spawn them if necessary
            var enemiesActive = false;
            foreach (var pair in room.ConnectedRooms)
            {
                if (pair.Value == myRoom) {
                    enemiesActive = true;
                    break;
                }
            }
            if (enemiesActive) 
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