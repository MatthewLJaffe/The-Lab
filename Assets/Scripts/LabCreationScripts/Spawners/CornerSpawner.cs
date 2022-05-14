using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace LabCreationScripts.Spawners
{
    [CreateAssetMenu(fileName = "CornerSpawner", menuName = "InteriorSpawners/CornerSpawner")]
    public class CornerSpawner : InteriorSpawner
    {
        [SerializeField] private Orientation[] orientations;
        [SerializeField] private bool stackAcross;
        
        public enum Corner
        {
            TopRight = 0,
            TopLeft = 1,
            BottomRight = 2,
            BottomLeft = 3
        }   

        [Serializable]
        public class Orientation
        {
            [SerializeField] public GameObject prefab;
            public Corner corner;
        }
        
        protected override bool TryToSpawn(BoundsInt bounds, Tilemap tMap, Transform roomTransform)
        {
            var availOrients = orientations.ToList();
            while (currentSpawns < targetSpawns && availOrients.Count > 0)
            {
                Vector3 spawnPos;
                var orientationToSpawn = orientations[Random.Range(0, orientations.Length)];
                prefab = orientationToSpawn.prefab;
                spawnCollider = FindSpawnCollider(prefab);
                var offset = (Vector3)spawnCollider.offset;
                var size = new Vector2(Mathf.RoundToInt(spawnCollider.size.x), Mathf.RoundToInt(spawnCollider.size.y));
                availOrients.Remove(orientationToSpawn);
                switch (orientationToSpawn.corner)
                {
                    case Corner.TopRight:
                        spawnPos = bounds.max - (Vector3)size / 2 - offset;
                        while (!SpawnClear(spawnPos))
                        {
                            if (!stackAcross) break;
                            spawnPos += (Vector3)Vector2.left * size.x;
                            if (spawnPos.x <= bounds.xMin + 1) break;
                        }
                        if (SpawnClear(spawnPos))
                            Spawn(spawnPos, roomTransform);
                        break;
                    case Corner.TopLeft:
                        spawnPos = new Vector3(bounds.xMin, bounds.yMax, 0) + new Vector3(size.x/2, -size.y /2, 0) - offset;
                        while (!SpawnClear(spawnPos))
                        {
                            if (!stackAcross) break;
                            spawnPos += (Vector3)Vector2.right * size.x;
                            if (spawnPos.x >= bounds.xMax - 1) break;
                        }
                        if (SpawnClear(spawnPos))
                            Spawn(spawnPos, roomTransform);
                        break;
                    case Corner.BottomRight:
                        spawnPos = new Vector3(bounds.xMax, bounds.yMin, 0) + new Vector3(-size.x/2, size.y /2, 0) - offset;
                        while (!SpawnClear(spawnPos))
                        {
                            if (!stackAcross) break;
                            spawnPos += (Vector3)Vector2.left * size.x;
                            if (spawnPos.x <= bounds.xMin + 1) break;
                        }
                        if (SpawnClear(spawnPos))
                            Spawn(spawnPos, roomTransform);
                        break;
                    case Corner.BottomLeft:
                        spawnPos = new Vector3(bounds.xMin, bounds.yMin, 0) + new Vector3(size.x/2, size.y /2, 0) - offset;
                        while (!SpawnClear(spawnPos))
                        {
                            if (!stackAcross) break;
                            spawnPos += (Vector3)Vector2.right * size.x;
                            if (spawnPos.x >= bounds.xMax -1) break;
                        }
                        if (SpawnClear(spawnPos))
                            Spawn(spawnPos, roomTransform);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return currentSpawns >= targetSpawns;
        }
    }
}