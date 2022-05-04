using System;
using System.Collections.Generic;
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
        
        protected override bool Spawn(BoundsInt bounds, Tilemap tMap, Transform roomTransform)
        {
            var orientationToSpawn = orientations[Random.Range(0, orientations.Length)];
            var size = new Vector2(Mathf.Round(spawnCollider.size.x), Mathf.Round(spawnCollider.size.y));
            Vector3 spawnPos;
            switch (orientationToSpawn.corner)
            {
                case Corner.TopRight:
                    spawnPos = bounds.max - (Vector3)size / 2;
                    while (!SpawnClear(spawnPos))
                    {
                        if (!stackAcross)
                            return false;
                        spawnPos += (Vector3)Vector2.left * size.x;
                        if (spawnPos.x <= bounds.xMin + 1) return true;
                    }
                    break;
                case Corner.TopLeft:
                    spawnPos = new Vector3(bounds.xMin, bounds.yMax, 0) + new Vector3(size.x/2, -size.y /2, 0);
                    while (!SpawnClear(spawnPos))
                    {
                        if (!stackAcross)
                            return false;
                        spawnPos += (Vector3)Vector2.right * size.x;
                        if (spawnPos.x >= bounds.xMax - 1) return true;
                    }
                    break;
                case Corner.BottomRight:
                    spawnPos = new Vector3(bounds.xMax, bounds.yMin, 0) + new Vector3(-size.x/2, size.y /2, 0);
                    while (!SpawnClear(spawnPos))
                    {
                        if (!stackAcross)
                            return false;
                        spawnPos += (Vector3)Vector2.left * size.x;
                        if (spawnPos.x <= bounds.xMin + 1) return true;
                    }
                    break;
                case Corner.BottomLeft:
                    spawnPos = new Vector3(bounds.xMin, bounds.yMin, 0) + new Vector3(size.x/2, size.y /2, 0);
                    while (!SpawnClear(spawnPos))
                    {
                        if (!stackAcross)
                            return false;
                        spawnPos += (Vector3)Vector2.right * size.x;
                        if (spawnPos.x >= bounds.xMax -1) return true;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Instantiate(orientationToSpawn.prefab, spawnPos, Quaternion.identity, roomTransform);
            currentSpawns++;
            return currentSpawns >= targetSpawns;
        }
    }
}