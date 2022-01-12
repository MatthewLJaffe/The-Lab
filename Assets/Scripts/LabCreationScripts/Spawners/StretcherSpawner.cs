using System;
using UnityEngine;

namespace LabCreationScripts.Spawners
{
    [CreateAssetMenu(fileName = "StretcherSpawner", menuName = "InteriorSpawners/StretcherSpawner")]
    public class StretcherSpawner : ObstacleSpawner
    {
        [SerializeField] private StretcherDirection direction;
        [SerializeField] private float spaceClear;
        
        [Serializable]
        public enum StretcherDirection
        {
            Horizontal,
            Vertical
        }
        
        protected override bool SpawnClear(Vector3 pos, BoundsInt bounds)
        {
            var fits = base.SpawnClear(pos, bounds);
            if (!fits) return false;
            pos += (Vector3)boxCollider.offset;
            var boxCastDir = direction == StretcherDirection.Horizontal ? Vector2.right : Vector2.up;
            //the stretcher must have at least spaceClear distance to roll either forwards or backwards
            return !Physics2D.BoxCast(pos, boxCollider.size, 0, boxCastDir, spaceClear, LayerMask.GetMask("Block")) ||
                   !Physics2D.BoxCast(pos, boxCollider.size, 0, -boxCastDir, spaceClear, LayerMask.GetMask("Block"));
        }
    }
}