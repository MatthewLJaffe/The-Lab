using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LabCreationScripts.ProceduralRooms;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LabCreationScripts
{
    public class Room
    {
        public Dictionary<Direction, Room> ConnectedRooms { get; } = new Dictionary<Direction, Room>();
        public BoundsInt RoomBounds { get; private set; }
        public int RoomId { get; private set; } = -1;
        public ProceduralRoom roomType;
        public GameObject roomGameObject;
        public readonly Room prevRoom;
        private static float dDoorOffset = .81f;


        public Room (Tilemap tMap, LabTiles labTiles, 
            RoomDimensions dimensions, Room[] rooms, FloorGenerator.RoomCategory[] roomCategories, Vector3Int doorPos, Direction dir, Room prevRoom, Transform parent, System.Action finish) 
        {
            if (rooms[rooms.Length - 1] != null) {
                finish.Invoke();
                return;
            }

            roomType = PickRoomType(roomCategories, rooms.Count(r => r == null), out var roomCategory);
            roomGameObject = DrawRoom(tMap, labTiles, dimensions, doorPos, dir, rooms, roomType, prevRoom, parent);
            if (!roomGameObject) return;
            roomCategory.amountToCreate--;
            roomCategory.roomInstances.Add(this);
            AddRoom(rooms);
            roomGameObject.name = $"Room {RoomId}";
            this.prevRoom = prevRoom;
            //recursive call
            SetConnectedRooms(tMap, labTiles, dimensions, rooms, roomCategories, dir, RoomBounds, parent, finish);
        }

        public Room(Room[] rooms, Vector3Int lDoor, Vector3Int rDoor, Vector3Int uDoor, Vector3Int dDoor, Transform floorParent)
        {
            roomGameObject = new GameObject($"Room {RoomId}");
            roomGameObject.transform.SetParent(floorParent);
            roomGameObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            RoomBounds = new BoundsInt(lDoor.x, dDoor.y, 0, (rDoor.x - lDoor.x) + 1, (uDoor.y - dDoor.y) + 1, 1);
            AddRoom(rooms);
        }

        private void AddRoom(Room[] rooms)
        {
            for (int i = 0; i < rooms.Length; i++) {
                if (rooms[i] == null) {
                    rooms[i] = this;
                    RoomId = i;
                    break;
                }
            }
        }
        /// <summary>
        /// Returns a gameobject representing the room if one is succesfully created and null otherwise
        /// </summary>
        /// <param name="tMap"></param>
        /// <param name="labTiles"></param>
        /// <param name="dim"></param>
        /// <param name="doorPos"></param>
        /// <param name="dir"></param>
        /// <param name="rooms"></param>
        /// <param name="roomType"></param>
        /// <param name="pRoom"></param>
        /// <returns></returns>
        private GameObject DrawRoom(Tilemap tMap, LabTiles labTiles, RoomDimensions dim, Vector3Int doorPos, Direction dir, 
            Room[] rooms, ProceduralRoom roomType, Room pRoom, Transform floorParent)
        {
            int width = Random.Range(roomType.minSize.x, roomType.maxSize.x);
            int height = Random.Range(roomType.minSize.y, roomType.maxSize.y);
            int hallLength = Random.Range(dim.minHallway, dim.maxHallway);
            RoomBounds = CreateRoomBounds(dir, doorPos, hallLength, width, height);
            if (!CheckIfClear(rooms))
                return null;

            var roomGO = new GameObject();
            roomGO.transform.SetParent(floorParent);
            roomGO.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            var roomTransform = roomGO.transform;
            Door prevRoomDoor;
            Door thisRoomDoor;
            switch (dir) 
            {
                case Direction.Up:
                    //door spawning
                    prevRoomDoor = Object.Instantiate(labTiles.uDoor, (Vector3)doorPos, Quaternion.identity, pRoom.roomGameObject.transform)
                        .GetComponent<Door>();
                    thisRoomDoor = Object.Instantiate(labTiles.dDoor, (Vector3)doorPos + Vector3.up * (hallLength - 1 + dDoorOffset), Quaternion.identity,
                            roomTransform).GetComponent<Door>();
                    prevRoomDoor.myRoom = pRoom;
                    thisRoomDoor.myRoom = this;
                    
                    //hallway spawning
                    for(int i = 0; i < hallLength; i++) {
                        tMap.SetTile(new Vector3Int(doorPos.x, doorPos.y + i, 0), labTiles.verticalHallRule);
                        tMap.SetTile(new Vector3Int(doorPos.x-1, doorPos.y + i, 0), labTiles.verticalHallRule);
                    }
                    //room spawning
                    for(int x = 0; x < width; x++) {
                        for(int y = 0; y < height; y++) {
                            if(!tMap.GetTile(new Vector3Int(doorPos.x - width / 2 + x, doorPos.y + hallLength - 1 + y, 0)))
                                tMap.SetTile(new Vector3Int(doorPos.x - width / 2 + x, doorPos.y + hallLength - 1 + y , 0), labTiles.roomRule);
                        }
                    }
                    break;
                
                case Direction.Right:
                    //door spawning
                    prevRoomDoor = Object.Instantiate(labTiles.rDoor, (Vector3)doorPos, Quaternion.identity, pRoom.roomGameObject.transform)
                        .GetComponent<Door>();
                    thisRoomDoor = Object.Instantiate(labTiles.lDoor, (Vector3)doorPos + Vector3.right * (hallLength), Quaternion.identity, roomTransform)
                        .GetComponent<Door>();
                    prevRoomDoor.myRoom = pRoom;
                    thisRoomDoor.myRoom = this;
                    
                    //hallway spawning
                    for (int i = 0; i < hallLength; i++) {
                        tMap.SetTile(new Vector3Int(doorPos.x + i, doorPos.y , 0), labTiles.horizontalHallRule);
                        tMap.SetTile(new Vector3Int(doorPos.x + i, doorPos.y-1, 0), labTiles.horizontalHallRule);
                    }
                    //room spawning
                    for (int x = 0; x < width; x++) {
                        for (int y = 0; y < height; y++) {
                            if (!tMap.GetTile(new Vector3Int(doorPos.x + hallLength - 1 + x, doorPos.y - height / 2 + y, 0)))
                                tMap.SetTile(new Vector3Int(doorPos.x + hallLength - 1 + x, doorPos.y - height/2 + y, 0), labTiles.roomRule);
                        }
                    }
                    break;

                case Direction.Down:
                    //door spawning
                    prevRoomDoor = Object.Instantiate(labTiles.dDoor, (Vector3)doorPos + Vector3.up*dDoorOffset, Quaternion.identity, pRoom.roomGameObject.transform)
                        .GetComponent<Door>();
                    thisRoomDoor = Object.Instantiate(labTiles.uDoor, (Vector3)doorPos + Vector3.down * (hallLength-1), Quaternion.identity, roomTransform)
                        .GetComponent<Door>();
                    prevRoomDoor.myRoom = pRoom;
                    thisRoomDoor.myRoom = this;
                    
                    //hallway spawning
                    for (int i = 0; i < hallLength; i++) {
                        tMap.SetTile(new Vector3Int(doorPos.x, doorPos.y - i, 0), labTiles.verticalHallRule);
                        tMap.SetTile(new Vector3Int(doorPos.x-1, doorPos.y - i, 0), labTiles.verticalHallRule);
                    }
                    //room spawning
                    for (int x = 0; x < width; x++) {
                        for (int y = 0; y < height; y++) {
                            if(!tMap.GetTile(new Vector3Int(doorPos.x - width / 2 + x, doorPos.y - hallLength + 1 - y, 0)))
                                tMap.SetTile(new Vector3Int(doorPos.x - width / 2 + x, doorPos.y - hallLength + 1 - y, 0), labTiles.roomRule);
                        }
                    }
                    break;

                case Direction.Left:
                    //door spawning
                    prevRoomDoor = Object.Instantiate(labTiles.lDoor, (Vector3)doorPos + Vector3.right, Quaternion.identity, pRoom.roomGameObject.transform)
                        .GetComponent<Door>();
                    thisRoomDoor = Object.Instantiate(labTiles.rDoor, (Vector3)doorPos + Vector3.left * (hallLength-1), Quaternion.identity, roomTransform)
                        .GetComponent<Door>();
                    prevRoomDoor.myRoom = pRoom;
                    thisRoomDoor.myRoom = this;
                    
                    //hallway spawning
                    for (int i = 0; i < hallLength; i++) {
                        tMap.SetTile(new Vector3Int(doorPos.x - i, doorPos.y, 0), labTiles.horizontalHallRule);
                        tMap.SetTile(new Vector3Int(doorPos.x - i, doorPos.y-1, 0), labTiles.horizontalHallRule);
                    }
                    //room spawning
                    for (int x = 0; x < width; x++) {
                        for (int y = 0; y < height; y++) {
                            if(!tMap.GetTile(new Vector3Int(doorPos.x - hallLength + 1 - x, doorPos.y - height / 2 + y, 0)))
                                tMap.SetTile(new Vector3Int(doorPos.x - hallLength + 1 - x, doorPos.y - height / 2 + y, 0), labTiles.roomRule);
                        }
                    }
                    break;
                
                default:
                    Debug.LogError("INVALID DIRECTION");
                    return null;
            }
            DrawWalls(tMap, labTiles);
            return roomGO;
        }

        private void DrawWalls(Tilemap tMap, LabTiles labTiles)
        {
            for(int x = RoomBounds.xMin; x < RoomBounds.xMax; x++) {
                for(int y = RoomBounds.yMax -2; y <= RoomBounds.yMax-1; y++) {
                    if(tMap.GetTile(new Vector3Int(x, y, 0)).Equals(labTiles.roomRule))
                        tMap.SetTile(new Vector3Int(x, y, 0), labTiles.wallRule);
                }
            }
        }
        private BoundsInt CreateRoomBounds(Direction dir, Vector3Int doorPos, int hallLength, int width, int height)
        {
            switch (dir)
            {
                case Direction.Up:
                    return new BoundsInt(doorPos.x - width / 2, doorPos.y + hallLength - 1, 0, width, height, 1);
                case Direction.Right:
                    return new BoundsInt(doorPos.x + hallLength - 1, doorPos.y - height / 2, 0, width, height, 1);
                case Direction.Down:
                    return new BoundsInt(doorPos.x - width / 2, doorPos.y - hallLength - height + 2, 0, width, height, 1);
                case Direction.Left:
                    return new BoundsInt(doorPos.x - hallLength - width + 2, doorPos.y - height / 2, 0, width, height, 1);
            }
            return new BoundsInt();
        }

        private bool CheckIfClear(Room[] rooms)
        {
            foreach(var other in rooms)
            {
                if (other == null)
                    return true;
                if ((RoomBounds.xMin > other.RoomBounds.xMax) || (RoomBounds.xMax < other.RoomBounds.xMin) ||
                    (RoomBounds.yMin > other.RoomBounds.yMax) || (RoomBounds.yMax < other.RoomBounds.yMin)) {
                    continue;
                }
                return false;
            }
            return false;
        }

        public static BoundsInt RoomBoundsToFloorBounds(BoundsInt roomBounds)
        {
            return new BoundsInt(roomBounds.xMin + 1, roomBounds.yMin + 1, 0, roomBounds.size.x - 2, roomBounds.size.y - 3, 0);
        }

        private async void SetConnectedRooms(Tilemap tMap, LabTiles labTiles, 
            RoomDimensions dimensions, Room[] rooms, FloorGenerator.RoomCategory[] roomCategories, Direction dir, BoundsInt RoomBounds, Transform parent, System.Action finish)
        {
            //set new rooms connected
            Dictionary<Direction, Vector3Int> potentialRooms = new Dictionary<Direction, Vector3Int>();
            List<Direction> allKeys = new List<Direction>();
            SetPotentialRooms(dir, RoomBounds, potentialRooms, allKeys);
            int numConnections = Random.Range(1, 4);
            for (int i = 0; i < numConnections; i++) {
                int index = Random.Range(0, allKeys.Count);
                Direction connectionDir = allKeys[index];
                allKeys.RemoveAt(index);
                await Task.Yield();
                ConnectedRooms.Add(connectionDir, 
                    new Room(tMap, labTiles, dimensions, rooms, roomCategories, potentialRooms[connectionDir], connectionDir, this, parent, finish));
            }
            var emptyRooms = ConnectedRooms.Where(pair => pair.Value.RoomId == -1);
            var emptyDirs = emptyRooms.ToDictionary
                (p => p.Key, p => p.Value);
            foreach (var key in emptyDirs.Keys) {
                ConnectedRooms.Remove(key);
            }
        }

        private void SetPotentialRooms (Direction dir, BoundsInt RoomBounds, 
            Dictionary<Direction, Vector3Int> potentialRooms, List<Direction> allKeys)
        {
            allKeys.Add(Direction.Up);
            allKeys.Add(Direction.Right);
            allKeys.Add(Direction.Down);
            allKeys.Add(Direction.Left);
            potentialRooms.Add(Direction.Up, new Vector3Int((int)RoomBounds.center.x, RoomBounds.max.y-1, 0));
            potentialRooms.Add(Direction.Right, new Vector3Int((int)RoomBounds.max.x-1, (int)RoomBounds.center.y, 0));
            potentialRooms.Add(Direction.Down, new Vector3Int((int)RoomBounds.center.x, (int)RoomBounds.min.y, 0));
            potentialRooms.Add(Direction.Left, new Vector3Int((int)RoomBounds.min.x, (int)RoomBounds.center.y, 0));
            if (dir == Direction.Up) {
                potentialRooms.Remove(Direction.Down);
                allKeys.Remove(Direction.Down);
            }
            else if (dir == Direction.Right) {
                potentialRooms.Remove(Direction.Left);
                allKeys.Remove(Direction.Left);
            }
            else if (dir == Direction.Down) {
                potentialRooms.Remove(Direction.Up);
                allKeys.Remove(Direction.Up);
            }
            else {
                potentialRooms.Remove(Direction.Right);
                allKeys.Remove(Direction.Right);
            }
        }

        private ProceduralRoom PickRoomType(FloorGenerator.RoomCategory[] roomCategories, int numRooms, out FloorGenerator.RoomCategory category)
        {
            var randValue = Random.Range(0, 1f);
            category = roomCategories.First(rc => rc.amountToCreate >= 1);
            foreach (var rc in roomCategories)
            {
                var weight = rc.amountToCreate / (float) numRooms;
                if (weight >= randValue && rc.amountToCreate != 0) {
                    category = rc;
                    break;
                }
                randValue -= weight;
            }
            
            var roomTypes = category.roomTypes;
            var lastRoomType =  roomTypes[roomTypes.Length - 1].proceduralRoom;
            randValue = Random.Range(0, 1f);
            foreach (var rt in roomTypes)
            {
                if (rt.weight >= randValue) {
                    return rt.proceduralRoom;
                }
                randValue -= rt.weight;
            }
            return lastRoomType;
        }
    }
}