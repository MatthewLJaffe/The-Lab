using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LabCreationScripts.Spawners
{
    [CreateAssetMenu(fileName = "CenterSpawner", menuName = "InteriorSpawners/CenterSpawner")]
    public class CenterSpawner : InteriorSpawner
    {
        public bool centerY = true;
        public bool centerX = true;
        protected override bool TryToSpawn(BoundsInt spawnBounds, Tilemap tMap, Transform roomTransform)
        {
            var size = spawnCollider.size;
            var offset = spawnCollider.offset;
            spawnBounds.position -= new Vector3Int(Mathf.RoundToInt(offset.x), Mathf.RoundToInt(offset.y), 0);
            var availableCoords = new List<Vector2Int>();
            for (var c = 0; c < targetSpawns; c++)
            {
                if (centerX && centerY)
                {
                    var spawnPos = spawnBounds.center;
                    if (SpawnClear(spawnBounds.center))
                        Spawn(new Vector3((int)spawnPos.x, (int)spawnPos.y, 0), roomTransform);
                }
                else if (centerX || centerY)
                {
                    if (centerX)
                        for (var y = spawnBounds.yMin + Mathf.RoundToInt(size.y/2); y <= spawnBounds.yMax - Mathf.RoundToInt(size.y/2); y++)
                            availableCoords.Add(new Vector2Int((int)spawnBounds.center.x, y));
                    else if (centerY)
                        for (var x = spawnBounds.xMin + Mathf.RoundToInt(size.x/2); x <= spawnBounds.xMax - Mathf.RoundToInt(size.x/2); x++)
                            availableCoords.Add(new Vector2Int(x, (int)spawnBounds.center.y));
                    while (availableCoords.Count > 0 && currentSpawns < targetSpawns)
                    {
                        var tryIndex = Random.Range(0, availableCoords.Count);
                        var spawnPos = new Vector3(availableCoords[tryIndex].x, availableCoords[tryIndex].y, 0);
                        availableCoords.RemoveAt(tryIndex);
                        if (SpawnClear(spawnPos)) {
                            Spawn(new Vector3(spawnPos.x, spawnPos.y, 0), roomTransform);
                            break;
                        }
                    }
                }
                else
                {
                    spawnBounds.position += new Vector3Int(Mathf.RoundToInt(offset.x), Mathf.RoundToInt(offset.y), 0);
                    return base.TryToSpawn(spawnBounds, tMap, roomTransform);
                }
            }
            return currentSpawns == targetSpawns;
        }
    }
}