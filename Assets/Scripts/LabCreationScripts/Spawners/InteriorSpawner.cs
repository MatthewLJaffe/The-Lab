using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace LabCreationScripts.Spawners
{
    [CreateAssetMenu(fileName = "InteriorSpawner", menuName = "InteriorSpawners/InteriorSpawner")]
    public class InteriorSpawner : ScriptableObject
    {
        public GameObject[] prefabs;
        [SerializeField] protected bool removeSpawnCollider;
        protected GameObject prefab;
        protected BoxCollider2D spawnCollider;
        protected int targetSpawns;
        protected int currentSpawns;

        public virtual void SpawnObjects(BoundsInt spawnBounds, Tilemap tMap, GameObject roomGameObject, int minSpawnsPerRoom, int maxSpawnsPerRoom)
        {
            prefab = prefabs[Random.Range(0, prefabs.Length)];
            targetSpawns = Random.Range(minSpawnsPerRoom, maxSpawnsPerRoom + 1);
            currentSpawns = 0;
            spawnCollider = FindSpawnCollider(prefab);
            if (!TryToSpawn(spawnBounds, tMap, roomGameObject.transform)) 
                Debug.LogError("Failed to spawn " + prefab.name + " in" + roomGameObject.name);
        }
        
        public virtual bool CheckSpawnObjects(BoundsInt spawnBounds, Tilemap tMap, GameObject roomGameObject, int minSpawnsPerRoom, int maxSpawnsPerRoom)
        {
            prefab = prefabs[Random.Range(0, prefabs.Length)];
            targetSpawns = Random.Range(minSpawnsPerRoom, maxSpawnsPerRoom + 1);
            currentSpawns = 0;
            spawnCollider = FindSpawnCollider(prefab);
            return TryToSpawn(spawnBounds, tMap, roomGameObject.transform);
        }

        protected virtual bool TryToSpawn(BoundsInt spawnBounds, Tilemap tMap, Transform roomTransform)
        {
            var size = spawnCollider.size;
            var offset = spawnCollider.offset;
            spawnBounds.position -= new Vector3Int(Mathf.RoundToInt(offset.x), Mathf.RoundToInt(offset.y), 0);
            var availableCoords = new List<Vector2Int>();
            for (var x = spawnBounds.xMin + 1 + Mathf.RoundToInt(size.x/2); x <= spawnBounds.xMax - Mathf.RoundToInt(size.x/2); x++)
                for (var y = spawnBounds.yMin + 1 + Mathf.RoundToInt(size.y/2); y <= spawnBounds.yMax - Mathf.RoundToInt(size.y/2); y++)
                    availableCoords.Add(new Vector2Int(x, y));
            while (availableCoords.Count > 0 && currentSpawns < targetSpawns)
            {
                var spawnPos = availableCoords[Random.Range(0, availableCoords.Count)];
                availableCoords.Remove(spawnPos);
                if (SpawnClear(new Vector3(spawnPos.x, spawnPos.y, 0)))
                    Spawn(new Vector3(spawnPos.x, spawnPos.y, 0), roomTransform);
            }
            return currentSpawns >= targetSpawns;
        }

        protected void Spawn(Vector3 spawnPos, Transform roomTransform)
        {
            currentSpawns++;
            var instance = Instantiate(prefab, new Vector3(spawnPos.x, spawnPos.y, 0), Quaternion.identity, roomTransform);
            if (removeSpawnCollider)
                FindSpawnCollider(instance).enabled = false;
        }

        protected virtual bool SpawnClear(Vector3 pos)
        {
            pos += (Vector3)spawnCollider.offset;
            var hit = Physics2D.BoxCast(
                pos, spawnCollider.size, 0, 
                Vector2.zero, 0, LayerMask.GetMask("Block", "Spawn", "Default", "BlockObjects"));
            return !hit;
        }

        protected BoxCollider2D FindSpawnCollider(GameObject go)
        {
            var parentCollider = go.GetComponent<BoxCollider2D>();
            if (parentCollider && (parentCollider.gameObject.layer == LayerMask.NameToLayer("Spawn") ||
                parentCollider.gameObject.layer == LayerMask.NameToLayer("BlockObjects")))
                return parentCollider;
            var collider =  go.GetComponentsInChildren<BoxCollider2D>()
                .FirstOrDefault(bc => 
                    bc.gameObject.layer == LayerMask.NameToLayer("Spawn") || bc.gameObject.layer == LayerMask.NameToLayer("BlockObjects"));
            if (collider)
                return collider;
            return go.GetComponent<BoxCollider2D>();
        }
    }
}