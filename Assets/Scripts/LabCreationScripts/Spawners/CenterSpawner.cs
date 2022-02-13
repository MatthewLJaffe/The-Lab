using UnityEngine;
using UnityEngine.Tilemaps;

namespace LabCreationScripts.Spawners
{
    [CreateAssetMenu(fileName = "CenterSpawner", menuName = "InteriorSpawners/CenterSpawner")]
    public class CenterSpawner : InteriorSpawner
    {
        protected override bool Spawn(BoundsInt spawnBounds, Tilemap tMap, Transform roomTransform)
        {
            //FIXME box collider offset could put spawn pos out of bounds
            var spawnPos = spawnBounds.center + (Vector3)boxCollider.offset;
            if (SpawnClear(spawnPos, spawnBounds))
            {
                currentSpawns++;
                Instantiate(prefab, spawnPos, Quaternion.identity, roomTransform);
            }
            return currentSpawns >= targetSpawns;
        }
    }
}