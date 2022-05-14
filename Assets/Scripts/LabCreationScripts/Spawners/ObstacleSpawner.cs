using UnityEngine;
using UnityEngine.Tilemaps;

namespace LabCreationScripts.Spawners
{
    [CreateAssetMenu(fileName = "ObstacleSpawner", menuName = "InteriorSpawners/ObstacleSpawner")]
    public class ObstacleSpawner : InteriorSpawner
    {
        [SerializeField] private Vector2 spaceAround;

        protected override bool TryToSpawn(BoundsInt bounds, Tilemap tMap, Transform roomTransform)
        {
            var size = spawnCollider.size;
            var xSpace = (int)(size.x / 2 + spaceAround.x);
            var ySpace= (int)(size.y / 2 + spaceAround.y);

            var spawnPos = tMap.CellToWorld(new Vector3Int(Random.Range(bounds.xMin + xSpace + 1, bounds.xMax - xSpace), 
                Random.Range(bounds.yMin + ySpace + 1, bounds.yMax - ySpace), 0));
            if (SpawnClear(spawnPos))
            {
                currentSpawns++;
                Instantiate(prefab, spawnPos, Quaternion.identity, roomTransform);
            }
            return currentSpawns >= targetSpawns;
        }

        protected override bool SpawnClear(Vector3 pos)
        {
            pos -= (Vector3) spawnCollider.offset;
            return !Physics2D.BoxCast(
                pos, spawnCollider.size + 2 * spaceAround, 0, 
                Vector2.zero, 0, LayerMask.GetMask("Block", "Spawn", "Default", "BlockObjects"));
        }
    }
}