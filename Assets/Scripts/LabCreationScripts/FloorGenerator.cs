using System;
using System.Collections.Generic;
using System.Linq;
using EntityStatsScripts;
using LabCreationScripts.ProceduralRooms;
using LabCreationScripts.Spawners;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace LabCreationScripts
{
    public class FloorGenerator : MonoBehaviour
    {
        [SerializeField] private PlayerStats playerStats;
        public float floorNumber;
        public static Action onFloorFinished = delegate {  };
        [SerializeField] private int numRooms = 20;
        [SerializeField] private GameObject roomPrefab;
        [SerializeField] private GameObject miniMap;
        [SerializeField] private GameObject miniMapRoomPrefab;
        [SerializeField] private GameObject miniMapHallwayPrefab;
        [SerializeField] private MiniMapRoom firstRoom;
        [SerializeField] private RoomCategory[] categories;
        [SerializeField] private RoomDimensions dimensions;
        [SerializeField] private LabTiles labTiles;
        [SerializeField] private Transform lDoor;
        [SerializeField] private Transform rDoor;
        [SerializeField] private Transform uDoor;
        [SerializeField] private Transform dDoor;
        public Transform floorParent;
        private Room[] _rooms;
        private Tilemap _tMap;
        private bool _floorFinished;
        private List<GameObject> _usedPrefabs;
        
        public enum CategoryName
        {
            NoLoot,
            Loot,
            Chest,
            Gun,
            End
        }

        [Serializable]
        public struct PostRoomFillSpawner
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
            public List<RoomWeight> roomTypes;
            public PostRoomFillSpawner[] postSpawns;
            public List<Room> roomInstances;
            public float totalProb = 1f;
            public bool dontRepeat;
            public bool dontRepeatPrefab;
            [Serializable]
            public struct RoomWeight
            {
                public ProceduralRoom proceduralRoom;
                public float weight;
            }
        }

        private void Awake()
        {
            _rooms = new Room[numRooms];
            _usedPrefabs = new List<GameObject>();
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

            var roomData = new RoomData(_tMap, labTiles, dimensions, _rooms, categories, floorParent, miniMap, roomPrefab,
                miniMapRoomPrefab, miniMapHallwayPrefab, _usedPrefabs, FinishFloor);
            var startRoom = new Room(roomData, lDoorPos, rDoorPos, uDoorPos, dDoorPos, floorParent);
            firstRoom.myRoom = startRoom;
            
           startRoom.ConnectedRooms.Add(Direction.Right, new Room(roomData, lDoorPos, Direction.Right, startRoom));
           startRoom.ConnectedRooms.Add(Direction.Left, new Room(roomData, lDoorPos, Direction.Left, startRoom));
           startRoom.ConnectedRooms.Add(Direction.Up, new Room(roomData, uDoorPos, Direction.Up, startRoom));
        }

        private void FinishFloor()
        {
            if(_floorFinished)
                return;
            playerStats.playerStatsDict[PlayerStats.StatType.CurrentFloor].CurrentValue = floorNumber;
            _floorFinished = true;
            foreach (var room in _rooms)
            {
                if (room.roomType)
                    room.roomType.FillRoom(room, _tMap, room.roomGameObject);
            }
            foreach (var cat in categories)
            {
                foreach (var postSpawn in cat.postSpawns)
                {
                    var spawnableRooms = new List<Room>(cat.roomInstances);
                    var amountSpawned = 0;
                    while (amountSpawned < postSpawn.amountToCreate && spawnableRooms.Count > 0)
                    {
                        var roomToSpawn = spawnableRooms[Random.Range(0, spawnableRooms.Count)];
                        spawnableRooms.Remove(roomToSpawn);
                        var spawnBounds = Room.RoomBoundsToFloorBounds(roomToSpawn.RoomBounds);
                        if (postSpawn.spawner.CheckSpawnObjects
                        (spawnBounds, _tMap, roomToSpawn.roomGameObject, postSpawn.minSpawnsPerRoom, postSpawn.maxSpawnsPerRoom))
                            amountSpawned++;
                    }
                }
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
