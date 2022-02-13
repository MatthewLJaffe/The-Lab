using LabCreationScripts.Spawners;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LabCreationScripts
{
    [CreateAssetMenu(fileName = "LockerSpawner", menuName = "InteriorSpawners/LockerSpawner")]
    public class LockerSpawner : InteriorSpawner
    {
        protected override bool Spawn(BoundsInt bounds, Tilemap tMap, Transform roomTransform)
        {
            Vector3 spawnPos = tMap.CellToWorld(new Vector3Int
                (Random.Range(bounds.xMin + 1, bounds.xMax - targetSpawns - 1), bounds.yMax, 0));
            for (int i = 0; i < targetSpawns; i++) {
                if (!SpawnClear(spawnPos, bounds))
                    return i > 0;
                Instantiate(prefab, spawnPos, Quaternion.identity, roomTransform);
                spawnPos.x += .5f;
            }
            return true;
        }
        
        protected override bool SpawnClear(Vector3 pos, BoundsInt bounds)
        {
            if (OverlapsDoor(pos, bounds)) return false;
            pos += (Vector3)boxCollider.offset;
            return !Physics2D.BoxCast(
                pos, boxCollider.size, 0, 
                Vector2.zero, 0, LayerMask.GetMask("Block", "Spawn"));
        }
    }
}