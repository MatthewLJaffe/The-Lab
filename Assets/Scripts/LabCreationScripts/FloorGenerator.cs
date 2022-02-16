using System;
using System.Collections.Generic;
using System.Linq;
using LabCreationScripts.ProceduralRooms;
using LabCreationScripts.Spawners;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace LabCreationScripts
{
    public class FloorGenerator : MonoBehaviour
    {
        public static Action onFloorFinished = delegate {  };
        [SerializeField] private RoomCategory[] categories;
        
        [SerializeField] private RoomDimensions dimensions;
        [SerializeField] private LabTiles labTiles;
        [SerializeField] private Transform lDoor;
        [SerializeField] private Transform rDoor;
        [SerializeField] private Transform uDoor;
        [SerializeField] private Transform dDoor;
        public Transform floorParent;
        private readonly Room[] _rooms = new Room[25];
        private Tilemap _tMap;
        private bool _floorFinished;
        
        public enum CategoryName
        {
            NoLoot,
            Locker,
            Chest,
            Gun,
            End
        }

        [Serializable]
        public struct PreRoomFillSpawner
        {
            public InteriorSpawner spawner;
            public int amountToCreate;
            public int minSpawnsPerRoom;
            public int maxSpawnsPerRoom;
        }
        
        [Serializable]
        public class RoomCategory
        {
            public CategoryName categoryName;
            public int amountToCreate;
            public RoomWeight[] roomTypes;
            public PreRoomFillSpawner[] preSpawns;
            public List<Room> roomInstances;

            [Serializable]
            public struct RoomWeight
            {
                public ProceduralRoom proceduralRoom;
                public float weight;
            }
        }
        
        private void Start()
        {
            _tMap = GetComponent<Tilemap>();
            var lDoorPos = _tMap.WorldToCell(lDoor.position);
            var rDoorPos = _tMap.WorldToCell(rDoor.position);
            var uDoorPos = _tMap.WorldToCell(uDoor.position);
            var dDoorPos = _tMap.WorldToCell(dDoor.position);
            foreach (var cat in categories)
                cat.roomInstances = new List<Room>();

            var startRoom = new Room(_rooms, lDoorPos, rDoorPos, uDoorPos, dDoorPos, floorParent);
            //startRoom.ConnectedRooms.Add(Direction.Left, 
               // new Room(_tMap, labTiles, dimensions, _rooms, categories, lDoorPos, Direction.Left, startRoom, floorParent, FinishFloor));
            //startRoom.ConnectedRooms.Add(Direction.Right, 
               // new Room(_tMap, labTiles, dimensions, _rooms, categories, rDoorPos, Direction.Right, startRoom, floorParent, FinishFloor));
            startRoom.ConnectedRooms.Add(Direction.Up, 
                new Room(_tMap, labTiles, dimensions, _rooms, categories, uDoorPos, Direction.Up, startRoom, floorParent, FinishFloor));
        }

        private void FinishFloor()
        {
            if(_floorFinished)
                return;
            _floorFinished = true;

            foreach (var cat in categories)
            {
                foreach (var preSpawn in cat.preSpawns)
                {
                    var spawnableRooms = new List<Room>(cat.roomInstances);
                    for (int c = 0; c < preSpawn.amountToCreate; c++)
                    {
                        var roomToSpawn = spawnableRooms[Random.Range(0, spawnableRooms.Count)];
                        spawnableRooms.Remove(roomToSpawn);
                        var spawnBounds = Room.RoomBoundsToFloorBounds(roomToSpawn.RoomBounds);
                        preSpawn.spawner.TrySpawn(spawnBounds, _tMap, roomToSpawn.roomGameObject, preSpawn.minSpawnsPerRoom, preSpawn.maxSpawnsPerRoom);
                    }
                }
            }
            foreach (var room in _rooms)
            {
                if (room.roomType)
                    room.roomType.FillRoom(room, _tMap, room.roomGameObject);
            }
            ConnectPreviousRooms();
            onFloorFinished.Invoke();
        }

        private void ConnectPreviousRooms()
        {
            foreach (var room in _rooms)
            {
                if (room.prevRoom == null) continue;
                
                if (room.prevRoom.ConnectedRooms.ContainsKey(Direction.Up) && room.prevRoom.ConnectedRooms[Direction.Up] == room)
                    room.ConnectedRooms.Add(Direction.Down, room.prevRoom);
                else if (room.prevRoom.ConnectedRooms.ContainsKey(Direction.Right) && room.prevRoom.ConnectedRooms[Direction.Right] == room)
                    room.ConnectedRooms.Add(Direction.Left, room.prevRoom);
                else if (room.prevRoom.ConnectedRooms.ContainsKey(Direction.Down) && room.prevRoom.ConnectedRooms[Direction.Down] == room)
                    room.ConnectedRooms.Add(Direction.Up, room.prevRoom);
                else if (room.prevRoom.ConnectedRooms.ContainsKey(Direction.Left) && room.prevRoom.ConnectedRooms[Direction.Left] == room)
                    room.ConnectedRooms.Add(Direction.Right, room.prevRoom);
            }
        }
    }
}
