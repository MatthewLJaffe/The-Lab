using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace LabCreationScripts.Spawners
{
    [CreateAssetMenu(fileName = "InteriorSpawner", menuName = "InteriorSpawners/InteriorSpawner")]
    public class InteriorSpawner : ScriptableObject
    {
        [SerializeField] protected GameObject[] prefabs;
        protected GameObject prefab;
        protected BoxCollider2D spawnCollider;
        protected int targetSpawns;
        protected int currentSpawns;

        public virtual void TrySpawn(BoundsInt spawnBounds, Tilemap tMap, GameObject roomGameObject, int minSpawnsPerRoom, int maxSpawnsPerRoom)
        {
            prefab = prefabs[Random.Range(0, prefabs.Length)];
            targetSpawns = Random.Range(minSpawnsPerRoom, maxSpawnsPerRoom + 1);
            currentSpawns = 0;
            spawnCollider = FindSpawnCollider();
            for (int i = 0; i < 1000; i++)
            {
                if (Spawn(spawnBounds, tMap, roomGameObject.transform))
                    return;
                prefab = prefabs[Random.Range(0, prefabs.Length)];
                spawnCollider = FindSpawnCollider();
            }
            Debug.LogError("Failed to spawn " + prefab.name + " in" + roomGameObject.name);
        }

        protected virtual bool Spawn(BoundsInt spawnBounds, Tilemap tMap, Transform roomTransform)
        {
            var size = spawnCollider.size;
            var offset = spawnCollider.offset;
            spawnBounds.position -= new Vector3Int(Mathf.RoundToInt(offset.x), Mathf.RoundToInt(offset.y), 0);
            var spawnPos = new Vector3Int(
                Random.Range(spawnBounds.xMin + Mathf.RoundToInt(size.x / 2), spawnBounds.xMax + 1 - Mathf.RoundToInt(size.x / 2)), 
                Random.Range(spawnBounds.yMin + Mathf.RoundToInt(size.y / 2), spawnBounds.yMax + 1 - Mathf.RoundToInt(size.y / 2)), 
                0);
            if (SpawnClear(spawnPos))
            {
                currentSpawns++;
                Instantiate(prefab, spawnPos, Quaternion.identity, roomTransform);
            }
            return currentSpawns >= targetSpawns;
        }

        protected virtual bool SpawnClear(Vector3 pos)
        {
            pos += (Vector3)spawnCollider.offset;
            var hit = Physics2D.BoxCast(
                pos, spawnCollider.size, 0, 
                Vector2.zero, 0, LayerMask.GetMask("Block", "Spawn", "Default", "BlockObjects"));
            return !hit;
        }

        protected BoxCollider2D FindSpawnCollider()
        {
            var collider =  prefab.GetComponentsInChildren<BoxCollider2D>()
                .FirstOrDefault(bc => bc.gameObject.layer == LayerMask.NameToLayer("Spawn") || bc.gameObject.layer == LayerMask.NameToLayer("BlockObjects"));
            if (collider)
                return collider;
            return prefab.GetComponent<BoxCollider2D>();
        }
    }
}