using System;
using System.Linq;
using LabCreationScripts.Spawners;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace LabCreationScripts.ProceduralRooms
{
    [CreateAssetMenu(fileName = "ProceduralRoom", menuName = "ProceduralRooms/ProceduralRoom")]
    public class ProceduralRoom : ScriptableObject
    {
        public Vector2Int minSize;
        public Vector2Int maxSize;
        [SerializeField] protected SpawnerData[] spawners;
        [SerializeField] protected bool lockRoom = true;

        [System.Serializable]
        public struct SpawnerData
        {
            public InteriorSpawner spawner;
            public int minSpawns;
            public int maxSpawns;
            public float spawnChance;
            public int spawnOrder;
        }

        public virtual void FillRoom(Room room, Tilemap tmap, GameObject roomGameObject)
        {
            foreach (var door in roomGameObject.GetComponentsInChildren<Door>()) {
                door.lockable = lockRoom;
            }
            var sortedSpawners = spawners.OrderBy(s => s.spawnOrder);
            foreach (var spawnData in sortedSpawners.ToArray()) 
            {
                if (Random.Range(0f, 1f) <= spawnData.spawnChance)
                {
                    var spawnBounds = Room.RoomBoundsToFloorBounds(room.RoomBounds);
                    spawnData.spawner.TrySpawn(spawnBounds, tmap, roomGameObject, spawnData.minSpawns, spawnData.maxSpawns);
                }
            }
        }
    }
}