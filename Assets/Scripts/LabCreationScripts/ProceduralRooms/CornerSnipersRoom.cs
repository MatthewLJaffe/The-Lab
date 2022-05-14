using LabCreationScripts.Spawners;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace LabCreationScripts.ProceduralRooms
{
    [CreateAssetMenu(fileName = "CornerSnipersRoom", menuName = "ProceduralRooms/CornerSnipersRoom")]
    public class CornerSnipersRoom : EnemyRoom
    {
        /*
        [SerializeField] private InteriorSpawner wallSpawner;


        public override void FillRoom(Room room, Tilemap tmap, GameObject roomGameObject)
        {
            base.FillRoom(room, tmap, roomGameObject);
            for (var c = 0; c < 4; c++) {
                var quadrantBounds = GetQuadrant(c, room.RoomBounds);
                wallSpawner.TrySpawn(quadrantBounds, tmap, roomGameObject, 1 ,1);
            }
        }

        protected override Vector2 PickSpawnPos(BoundsInt bounds, BoxCollider2D spawnBoxCollider)
        {
            var pos = new Vector2(Random.Range(bounds.center.x - 4, bounds.center.x + 5),
                Random.Range(bounds.center.y - 4, bounds.center.y + 5));
            for (var c = 0; c < 1000; c++)
            {
                pos = new Vector2(Random.Range(bounds.xMin, bounds.xMax + 1),
                    Random.Range(bounds.yMin, bounds.yMax + 1));
                if (CanSpawnAtPos(pos, spawnBoxCollider))
                    return pos;
            }
            Debug.LogError("COULD NOT FIND SPAWN POS FOR ENEMY ");
            return pos;
        }

        private BoundsInt GetQuadrant(int quadNum, BoundsInt roomBounds)
        {
            var bounds = Room.RoomBoundsToFloorBounds(roomBounds);
            switch (quadNum % 4)
            {
                case 0:
                    return new BoundsInt((int)bounds.center.x, (int)bounds.center.y, 0, bounds.size.x / 2, bounds.size.y / 2, 0);
                case 1:
                    return new BoundsInt(bounds.xMin, (int)bounds.center.y, 0, bounds.size.x / 2, bounds.size.y / 2, 0);
                case 2:
                    return new BoundsInt(bounds.xMin, bounds.yMin, 0, bounds.size.x / 2, bounds.size.y / 2, 0);
                case 3:
                    return new BoundsInt((int)bounds.center.x, bounds.yMin, 0, bounds.size.x / 2, bounds.size.y / 2, 0);
            }
            Debug.LogError("quadNum out of bounds");
            return new BoundsInt();
        }
        */
    }
}