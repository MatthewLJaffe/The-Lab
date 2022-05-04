using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LabCreationScripts.Spawners
{
    [CreateAssetMenu(fileName = "SpikeLineSpawner", menuName = "InteriorSpawners/SpikeLineSpawner")]
    public class SpikeLineSpawner : InteriorSpawner
    {
        [SerializeField] private int wallGap;
        private BoundsInt _tileBounds;
        [SerializeField] private GameObject spikeParent;
        [SerializeField] private LineDirection lineDirection;
        
        [System.Serializable]
        private enum LineDirection
        {
            Horizontal,
            Vertical
        }
        
        public override void TrySpawn(BoundsInt spawnBounds, Tilemap tMap, GameObject roomGameObject, int minSpawnsPerRoom,
            int maxSpawnsPerRoom)
        {
            prefab = prefabs[Random.Range(0, prefabs.Length)];
            targetSpawns = Random.Range(minSpawnsPerRoom, maxSpawnsPerRoom + 1);
            currentSpawns = 0;
            for (int i = 0; i < 1000; i++)
            {
                if (Spawn(spawnBounds, tMap, roomGameObject.transform))
                    return;
            }
            Debug.LogError("Failed to spawn " + prefab.name + " in" + roomGameObject.name);
        }

        protected override bool Spawn(BoundsInt spawnBounds, Tilemap tMap, Transform roomTransform)
        {
            BoundsInt lineBounds;
            if (lineDirection == LineDirection.Horizontal)
            {
                lineBounds = new BoundsInt(spawnBounds.xMin + wallGap, Random.Range(spawnBounds.yMin + wallGap, spawnBounds.yMax - 3 - wallGap), 0,
                    spawnBounds.size.x - 2 * wallGap, 3, 0);
            }
            else
            {
                lineBounds = new BoundsInt(Random.Range(spawnBounds.xMin, spawnBounds.xMax - 3),
                    spawnBounds.yMin + wallGap, 0, 3, spawnBounds.size.y - 2 * wallGap, 0);
            }
            if (!Physics2D.BoxCast(lineBounds.center,new Vector2(lineBounds.size.x, lineBounds.size.y), 0,
                Vector2.zero, 0, LayerMask.GetMask("Block", "Spawn", "Default", "BlockObjects")))
            {
                var parentInstance = Instantiate(spikeParent, lineBounds.center, Quaternion.identity, roomTransform);
                parentInstance.GetComponent<BoxCollider2D>().size = new Vector2(lineBounds.size.x - .1f, lineBounds.size.y - .1f);
                if (lineDirection == LineDirection.Horizontal) {
                    for (int c = 0; c < lineBounds.size.x; c++) {
                        Instantiate(prefabs[0], new Vector3(lineBounds.xMin + .5f + c, lineBounds.center.y, 0),
                            Quaternion.identity, parentInstance.transform);
                    }
                }
                else {
                    for (int c = 0; c < lineBounds.size.y; c++) {
                        Instantiate(prefabs[0], new Vector3(lineBounds.center.x, lineBounds.yMin + .5f + c, 0),
                            Quaternion.identity, parentInstance.transform);
                    }
                }
                currentSpawns++;
            }
            return currentSpawns >= targetSpawns;
        }
    }
}