using System;
using UnityEngine;

namespace LabCreationScripts.Spawners
{
    [CreateAssetMenu(fileName = "StretcherSpawner", menuName = "InteriorSpawners/StretcherSpawner")]
    public class StretcherSpawner : InteriorSpawner
    {
        [SerializeField] private StretcherDirection direction;
        [SerializeField] private float spaceClear;
        
        [Serializable]
        public enum StretcherDirection
        {
            Horizontal,
            Vertical
        }
        
        protected override bool SpawnClear(Vector3 pos)
        {
            var fits = base.SpawnClear(pos);
            if (!fits) return false;
            pos += (Vector3)spawnCollider.offset;
            var boxCastDir = direction == StretcherDirection.Horizontal ? Vector2.right : Vector2.up;
            //the stretcher must have at least spaceClear distance to roll either forwards or backwards
            return !Physics2D.BoxCast(pos, spawnCollider.size, 0, boxCastDir, spaceClear, 
                       LayerMask.GetMask("Block", "Spawn", "Default", "BlockObjects")) ||
                   !Physics2D.BoxCast(pos, spawnCollider.size, 0, -boxCastDir, spaceClear, 
                       LayerMask.GetMask("Block", "Spawn", "Default", "BlockObjects"));
        }
    }
}