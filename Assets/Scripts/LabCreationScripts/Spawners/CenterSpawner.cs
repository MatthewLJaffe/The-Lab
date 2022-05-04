using UnityEngine;
using UnityEngine.Tilemaps;

namespace LabCreationScripts.Spawners
{
    [CreateAssetMenu(fileName = "CenterSpawner", menuName = "InteriorSpawners/CenterSpawner")]
    public class CenterSpawner : InteriorSpawner
    {
        public bool centerY = true;
        public bool centerX = true;
        protected override bool Spawn(BoundsInt spawnBounds, Tilemap tMap, Transform roomTransform)
        {
            if (centerX && centerY)
            {
                var spawnPos = new Vector3(Random.Range(spawnBounds.xMin, spawnBounds.xMax), Random.Range(spawnBounds.yMin, spawnBounds.yMax), 0);
                if (SpawnClear(spawnPos))
                {
                    currentSpawns++;
                    Instantiate(prefab, spawnPos, Quaternion.identity, roomTransform);
                }
            }
            else if (centerX)
            {
                
            }
            var xPos = centerX ? spawnBounds.center.x : Random.Range(spawnBounds.xMin, spawnBounds.xMax + 1);
            var yPos = centerY ? spawnBounds.center.y : Random.Range(spawnBounds.yMin, spawnBounds.yMax + 1);

            return currentSpawns >= targetSpawns;
        }
    }
}