using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace LabCreationScripts.Spawners
{
    [CreateAssetMenu(fileName = "SideSpawner", menuName = "InteriorSpawners/SideSpawner")]
    public class SideSpawner : InteriorSpawner
    {
        [SerializeField] private SidePrefab[] sides;

       [Serializable]
        public struct SidePrefab
        {
            public Side side;
            public GameObject prefab;
        }
        
        public enum Side
        {
            Left,
            Top,
            Right,
            Bottom
        }
        
        protected override bool TryToSpawn(BoundsInt bounds, Tilemap tMap, Transform roomTransform)
        {
            var sidesList = sides.ToList();
            while (sidesList.Count > 0 && currentSpawns < targetSpawns)
            {
                var sideToSpawn = sidesList[Random.Range(0, sidesList.Count)];
                sidesList.Remove(sideToSpawn);
                prefab = sideToSpawn.prefab;
                spawnCollider = FindSpawnCollider(prefab);
                var size = new Vector2Int(Mathf.RoundToInt(spawnCollider.size.x), Mathf.RoundToInt(spawnCollider.size.y));
                var offset = spawnCollider.offset;
                var spawnPositions = new List<Vector2Int>();
                switch (sideToSpawn.side)
                {
                    case Side.Left:
                        for (var y = bounds.yMin + Mathf.RoundToInt(size.y/2f) + 1; y <= bounds.yMax - Mathf.RoundToInt(size.y/2f); y++)
                            spawnPositions.Add(new Vector2Int(bounds.xMin, y));
                        while (spawnPositions.Count > 0 && currentSpawns < targetSpawns)
                        {
                            var tryIdx = Random.Range(0, spawnPositions.Count);
                            var spawnPos = (Vector2) spawnPositions[tryIdx];
                            spawnPositions.RemoveAt(tryIdx);
                            if (SpawnClear(spawnPos))
                                Spawn(spawnPos, roomTransform);
                        }
                        break;
                    case Side.Top:
                        for (var x = bounds.xMin + Mathf.RoundToInt(size.x/2f) + 1; x <= bounds.xMax - Mathf.RoundToInt(size.x/2f); x++)
                            spawnPositions.Add(new Vector2Int(x, bounds.yMax));
                        while (spawnPositions.Count > 0 && currentSpawns < targetSpawns)
                        {
                            var tryIdx = Random.Range(0, spawnPositions.Count);
                            var spawnPos = (Vector2) spawnPositions[tryIdx];
                            spawnPositions.RemoveAt(tryIdx);
                            if (SpawnClear(spawnPos))
                                Spawn(spawnPos, roomTransform);
                        }
                        break;
                    case Side.Right:
                        for (var y = bounds.yMin + Mathf.RoundToInt(size.y/2f) + 1; y <= bounds.yMax - Mathf.RoundToInt(size.y/2f); y++)
                            spawnPositions.Add(new Vector2Int(bounds.xMax, y));
                        while (spawnPositions.Count > 0 && currentSpawns < targetSpawns)
                        {
                            var tryIdx = Random.Range(0, spawnPositions.Count);
                            var spawnPos = (Vector2) spawnPositions[tryIdx];
                            spawnPositions.RemoveAt(tryIdx);
                            if (SpawnClear(spawnPos))
                                Spawn(spawnPos, roomTransform);
                        }
                        break;
                    case Side.Bottom:
                        for (var x = bounds.xMin + Mathf.RoundToInt(size.x/2f) + 1; x <= bounds.xMax - Mathf.RoundToInt(size.x/2f); x++)
                            spawnPositions.Add(new Vector2Int(x, bounds.yMin));
                        while (spawnPositions.Count > 0 && currentSpawns < targetSpawns)
                        {
                            var tryIdx = Random.Range(0, spawnPositions.Count);
                            var spawnPos = (Vector2) spawnPositions[tryIdx];
                            spawnPositions.RemoveAt(tryIdx);
                            if (SpawnClear(spawnPos))
                                Spawn(spawnPos, roomTransform);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return currentSpawns == targetSpawns;
        }
    }
}