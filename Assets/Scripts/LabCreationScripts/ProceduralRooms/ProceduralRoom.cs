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
        
        [Serializable]
        public enum HorizontalConstraints
        {
            None,
            LeftHalf,
            RightHalf
        }
        
        [Serializable]
        public enum VerticalConstraints
        {
            None,
            TopHalf,
            BottomHalf
        }

        [Serializable]
        public struct SpawnerData
        {
            public InteriorSpawner spawner;
            public int minSpawns;
            public int maxSpawns;
            public float spawnChance;
            public int spawnOrder;
            public VerticalConstraints vertConstraints;
            public HorizontalConstraints horizConstraints;
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
                    var roomBounds = Room.RoomBoundsToFloorBounds(room.RoomBounds);
                    var spawnBounds = new BoundsInt(roomBounds.position, roomBounds.size);
                    if (spawnData.vertConstraints == VerticalConstraints.TopHalf)
                        spawnBounds.yMin += spawnBounds.size.y / 2;
                    if (spawnData.vertConstraints == VerticalConstraints.BottomHalf)
                        spawnBounds.yMax = spawnBounds.yMin + spawnBounds.size.y / 2;
                    if (spawnData.horizConstraints == HorizontalConstraints.LeftHalf)
                        spawnBounds.xMax = spawnBounds.xMin + spawnBounds.size.x / 2;
                    if (spawnData.horizConstraints == HorizontalConstraints.RightHalf)
                        spawnBounds.xMin += spawnBounds.size.x / 2;
                    spawnData.spawner.TrySpawn(spawnBounds, tmap, roomGameObject, spawnData.minSpawns, spawnData.maxSpawns);
                }
            }
        }
    }
}