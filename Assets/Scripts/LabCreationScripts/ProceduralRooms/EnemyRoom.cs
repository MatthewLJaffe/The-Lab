using System;
using EnemyScripts;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace LabCreationScripts.ProceduralRooms
{
    [CreateAssetMenu(fileName = "EnemyRoom", menuName = "ProceduralRooms/EnemyRoom")]
    public class EnemyRoom : ProceduralRoom
    {
        [SerializeField] protected GameObject enemyHandlerPrefab;
        [SerializeField] protected int minEnemies;
        [SerializeField] protected int maxEnemies;
        [SerializeField] protected EnemyWeight[] weightedEnemies;
        protected EnemyHandler enemyHandler;
        
        [Serializable]
        public struct EnemyWeight
        {
            public GameObject enemy;
            public float weight;

        }
        public override void FillRoom(Room room, Tilemap tmap, GameObject roomGameObject)
        {
            base.FillRoom(room, tmap, roomGameObject);
            CreateEnemies(room, roomGameObject);
            var enemyHandlerGO = Instantiate(enemyHandlerPrefab, Vector3.zero, Quaternion.identity,
                roomGameObject.transform);
            enemyHandler = enemyHandlerGO.GetComponent<EnemyHandler>();
            enemyHandler.myRoom = room;
        }

        protected virtual void CreateEnemies(Room room, GameObject roomGameObject)
        {
            var numEnemies = Random.Range(minEnemies, maxEnemies + 1);
            for (int i = 0; i < numEnemies; i++) {
                var enemy = Instantiate(PickEnemy(), roomGameObject.transform).GetComponent<Enemy>();
                enemy.gameObject.SetActive(false);
                var roomBounds = room.RoomBounds;
                var bounds = new BoundsInt(roomBounds.xMin + 2, roomBounds.yMin + 2, 0, roomBounds.size.x - 4,
                    roomBounds.size.y - 5, 0);
                enemy.SpawnPos = PickSpawnPos(bounds, enemy.spawnCollider);
                enemy.transform.position = enemy.SpawnPos;
            }
        }

        protected virtual GameObject PickEnemy()
        {
            var randValue = Random.Range(0f, 1f);
            foreach (var ew in weightedEnemies) {
                if (ew.weight >= randValue)
                    return ew.enemy;
                randValue -= ew.weight;
            }
            return weightedEnemies[weightedEnemies.Length - 1].enemy;
        }

        protected virtual Vector2 PickSpawnPos(BoundsInt bounds, BoxCollider2D spawnCollider)
        {
            var pos = new Vector2(Random.Range((float)bounds.xMin, bounds.xMax + 1),
                Random.Range((float)bounds.yMin, bounds.yMax + 1));
            for (var c = 0; c < 1000; c++)
            {
                pos = new Vector2(Random.Range((float)bounds.xMin, bounds.xMax + 1),
                    Random.Range((float)bounds.yMin, bounds.yMax + 1));
                if (CanSpawnAtPos(pos, spawnCollider))
                    return pos;
            }
            Debug.LogError("COULD NOT FIND SPAWN POS FOR ENEMY");
            return pos;
        }

        protected virtual bool CanSpawnAtPos(Vector2 pos, BoxCollider2D enemyCollider)
        {
            return !Physics2D.BoxCast(pos + enemyCollider.offset, enemyCollider.size, 0f, Vector2.zero, 0f,
                LayerMask.GetMask("Block", "Spawn", "Default"));
        }
    }
}