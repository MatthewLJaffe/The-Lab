using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace LabCreationScripts.Spawners
{
    [CreateAssetMenu(fileName = "InteriorSpawner", menuName = "InteriorSpawners/InteriorSpawner")]
    public class InteriorSpawner : ScriptableObject
    {
        [SerializeField] private GameObject[] prefabs;
        protected GameObject prefab;
        protected BoxCollider2D boxCollider;
        protected int targetSpawns;
        protected int currentSpawns;
        protected const float VertDoorWidth = 2f;
        protected const float VertDoorHeight = 1f;
        protected const float HorizDoorWidth = 1f;
        protected const float HorizDoorHeight = 2f;
        
        public virtual void TrySpawn(BoundsInt spawnBounds, Tilemap tMap, GameObject roomGameObject, int minSpawnsPerRoom, int maxSpawnsPerRoom)
        {
            prefab = prefabs[Random.Range(0, prefabs.Length)];
            targetSpawns = Random.Range(minSpawnsPerRoom, maxSpawnsPerRoom + 1);
            currentSpawns = 0;
            boxCollider = prefab.GetComponent<BoxCollider2D>();
            for (int i = 0; i < 1000; i++)
            {
                if (Spawn(spawnBounds, tMap, roomGameObject.transform))
                    return;
                prefab = prefabs[Random.Range(0, prefabs.Length)];
            }
            Debug.LogError("Failed to spawn " + prefab.name + " in" + roomGameObject.name);
        }

        protected virtual bool Spawn(BoundsInt spawnBounds, Tilemap tMap, Transform roomTransform)
        {
            var size = boxCollider.size;
            //FIXME box collider offset could put spawn pos out of bounds
            var spawnPos = tMap.CellToWorld(new Vector3Int(Random.Range(spawnBounds.xMin + (int)size.x / 2, spawnBounds.xMax + 1 - (int)size.x / 2), 
                Random.Range(spawnBounds.yMin + (int)size.y / 2, spawnBounds.yMax + 1 - (int)size.y / 2), 0)) + (Vector3)boxCollider.offset;
            if (SpawnClear(spawnPos, spawnBounds))
            {
                currentSpawns++;
                Instantiate(prefab, spawnPos, Quaternion.identity, roomTransform);
            }
            return currentSpawns >= targetSpawns;
        }

        protected bool OverlapsDoor(Vector3 pos, BoundsInt bounds)
        {
            var center = bounds.center;
            var size = boxCollider.size;
            pos += (Vector3)boxCollider.offset;
            return Overlaps(pos, size, new Vector2Int((int)center.x, bounds.yMax), new Vector2(VertDoorWidth, VertDoorHeight)) ||
                   Overlaps(pos, size, new Vector2Int((int)center.x, bounds.yMin), new Vector2(VertDoorWidth, VertDoorHeight)) ||
                   Overlaps(pos, size, new Vector2Int(bounds.xMax, (int)center.y),new Vector2(HorizDoorWidth, HorizDoorHeight)) ||
                   Overlaps(pos, size, new Vector2Int(bounds.xMin, (int)center.y), new Vector2(HorizDoorWidth, HorizDoorHeight));
        }

        private bool Overlaps(Vector2 pos1, Vector2 size1, Vector2 pos2, Vector2 size2)
        {
            return !(pos1.y + size1.y/2 < pos2.y - size2.y/2) && !(pos2.y + size2.y/2 < pos1.y - size1.y/2) &&
                   !(pos1.x + size1.x/2 < pos2.x - size2.x/2) && !(pos2.x + size2.x/2 < pos1.x - size1.x/2);
        }

        protected virtual bool SpawnClear(Vector3 pos, BoundsInt bounds)
        {

            if (OverlapsDoor(pos, bounds)) return false;
            pos += (Vector3)boxCollider.offset;
            return !Physics2D.BoxCast(
                pos, boxCollider.size, 0, 
                Vector2.zero, 0, LayerMask.GetMask("Block", "Spawn", "Default"));
        }
    }
}