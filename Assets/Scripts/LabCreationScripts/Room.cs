﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LabCreationScripts.ProceduralRooms;
using LabCreationScripts.Spawners;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace LabCreationScripts
{
    public class Room
    {
        public Dictionary<Direction, Room> ConnectedRooms { get; } = new Dictionary<Direction, Room>();
        public BoundsInt RoomBounds { get; private set; }
        public int RoomId { get; private set; } = -1;
        public ProceduralRoom roomType;
        public GameObject roomGameObject;
        public MiniMapRoom miniMapRoom;
        public readonly Room prevRoom;
        private static float dDoorOffset = .81f;


        public Room (RoomData roomData, Vector3Int doorPos, Direction dir, Room prevRoom) 
        {
            if (roomData.rooms[roomData.rooms.Length - 1] != null) {
                roomData.finish.Invoke();
                return;
            }
            var roomWeight = PickRoomType(roomData.roomCategories, roomData.rooms.Count(r => r == null), out var roomCategory);
            roomType = roomWeight.proceduralRoom;
            roomGameObject = DrawRoom(roomData, doorPos, dir, roomType, prevRoom);
            if (!roomGameObject) return;
            roomCategory.amountToCreate--;
            
            if (roomCategory.dontRepeat || roomCategory.dontRepeatPrefab)
            {
                roomCategory.totalProb -= roomWeight.weight;
                roomCategory.roomTypes.Remove(roomWeight);
            }
            if (roomCategory.dontRepeatPrefab)
            {
                //find other room category
                FloorGenerator.RoomCategory categoryToRemove;
                if (roomCategory.categoryName == FloorGenerator.CategoryName.Loot)
                    categoryToRemove = roomData.roomCategories.First(c => c.categoryName == FloorGenerator.CategoryName.NoLoot);
                else
                    categoryToRemove = roomData.roomCategories.First(c => c.categoryName == FloorGenerator.CategoryName.Loot);
                //find room prefab to look for in other room category
                var roomPrefab = roomWeight.proceduralRoom.spawners.First(s =>
                    s.spawnOrder == 0 && s.spawnChance > .99f && s.potentialSpawns.Length == 1 &&
                    s.potentialSpawns[0].spawner.GetType() == typeof(CenterSpawner)).potentialSpawns[0].spawner.prefabs[0];
                //search for room prefab in other room category
                FloorGenerator.RoomCategory.RoomWeight roomWeightToRemove = roomWeight;
                var found = false;
                //cry bc you over engineered a problem that never existed
                foreach (var rw in categoryToRemove.roomTypes) 
                {
                    if (found)
                        break;
                    foreach (var spawner in rw.proceduralRoom.spawners)
                    {
                        if (found)
                            break;
                        foreach (var s in spawner.potentialSpawns)
                        {
                            if (found)
                                break;
                            if (s.spawner && s.spawner.prefabs.Length > 0 && s.spawner.prefabs[0] == roomPrefab)
                            {
                                roomWeightToRemove = rw;
                                found = true;
                            }
                        }
                    }
                }
                //remove roomType from other room category
                categoryToRemove.totalProb -= roomWeightToRemove.weight;
                categoryToRemove.roomTypes.Remove(roomWeightToRemove);
            }
            roomCategory.roomInstances.Add(this);
            AddRoom(roomData.rooms);
            roomGameObject.name = $"Room {RoomId}";
            roomGameObject.GetComponent<RoomInstance>().myRoom = this;
            this.prevRoom = prevRoom;
            //Reveal the last room
            if (RoomId == roomData.rooms.Length - 1) {
                miniMapRoom.RevealRoom();
            }
            //recursive call
            SetConnectedRooms(roomData, dir);
        }

        public Room(RoomData roomData, Vector3Int lDoor, Vector3Int rDoor, Vector3Int uDoor, Vector3Int dDoor, Transform floorParent)
        {
            roomGameObject = Object.Instantiate(roomData.roomPrefab, floorParent);
            roomGameObject.GetComponent<RoomInstance>().myRoom = this;
            roomGameObject.name = $"Room {RoomId}";
            roomGameObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            RoomBounds = new BoundsInt(lDoor.x, dDoor.y, 0, (rDoor.x - lDoor.x) + 1, (uDoor.y - dDoor.y) + 1, 1);
            AddRoom(roomData.rooms);
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
        /// <param name="roomData"></param>
        /// <param name="doorPos"></param>
        /// <param name="dir"></param>
        /// <param name="rooms"></param>
        /// <param name="proceduralRoom"></param>
        /// <param name="pRoom"></param>
        /// <param name="miniMap"></param>
        /// <param name="miniMapRoomPrefab"></param>
        /// <param name="floorParent"></param>
        /// <returns></returns>
        private GameObject DrawRoom(RoomData roomData, Vector3Int doorPos, Direction dir, 
           ProceduralRoom proceduralRoom, Room pRoom)
        {
            int width = Random.Range(proceduralRoom.minSize.x, proceduralRoom.maxSize.x);
            int height = Random.Range(proceduralRoom.minSize.y, proceduralRoom.maxSize.y);
            int hallLength = Random.Range(roomData.dimensions.minHallway, roomData.dimensions.maxHallway);
            RoomBounds = CreateRoomBounds(dir, doorPos, hallLength, width, height);
            if (!CheckIfClear(roomData.rooms))
                return null;

            //create miniMap room
            miniMapRoom = Object.Instantiate
                (roomData.miniMapRoomPrefab, RoomBounds.center, Quaternion.identity, roomData.miniMap.transform).GetComponent<MiniMapRoom>();
            miniMapRoom.transform.localScale = new Vector3(width, height, 1);
            miniMapRoom.myRoom = this;
            
            //create roomGameObject
            var roomGO = Object.Instantiate(roomData.roomPrefab, roomData.parent);
            roomGO.transform.SetPositionAndRotation(RoomBounds.center, Quaternion.identity);
            var roomTransform = roomGO.transform;
            Door prevRoomDoor;
            Door thisRoomDoor;
            switch (dir) 
            {
                case Direction.Up:
                    //door spawning
                    prevRoomDoor = Object.Instantiate(roomData.labTiles.uDoor, (Vector3)doorPos, Quaternion.identity, pRoom.roomGameObject.transform)
                        .GetComponent<Door>();
                    thisRoomDoor = Object.Instantiate(roomData.labTiles.dDoor, (Vector3)doorPos + Vector3.up * (hallLength - 1 + dDoorOffset), Quaternion.identity,
                            roomTransform).GetComponent<Door>();
                    prevRoomDoor.myRoom = pRoom;
                    thisRoomDoor.myRoom = this;
                    
                    //room spawning
                    for(int x = 0; x < width; x++) {
                        for(int y = 0; y < height; y++) {
                            roomData.tMap.SetTile(new Vector3Int(doorPos.x - width / 2 + x, doorPos.y + hallLength - 1 + y , 0), roomData.labTiles.roomRule);
                        }
                    }
                    
                    //hallway spawning
                    for(int i = 0; i < hallLength; i++) {
                        roomData.tMap.SetTile(new Vector3Int(doorPos.x, doorPos.y + i, 0), roomData.labTiles.verticalHallRule);
                        roomData.tMap.SetTile(new Vector3Int(doorPos.x-1, doorPos.y + i, 0), roomData.labTiles.verticalHallRule);
                    }
                    var miniMapHallwayU = Object.Instantiate(roomData.miniMapHallwayPrefab, new Vector3(doorPos.x, doorPos.y + hallLength/2, 0),
                        Quaternion.identity, roomData.miniMap.transform);
                    miniMapHallwayU.transform.localScale = new Vector3(2, hallLength, 1);
                    miniMapRoom.hallwaySR = miniMapHallwayU.GetComponent<SpriteRenderer>();
                    
                    break;
                
                case Direction.Right:
                    //door spawning
                    prevRoomDoor = Object.Instantiate(roomData.labTiles.rDoor, (Vector3)doorPos, Quaternion.identity, pRoom.roomGameObject.transform)
                        .GetComponent<Door>();
                    thisRoomDoor = Object.Instantiate(roomData.labTiles.lDoor, (Vector3)doorPos + Vector3.right * (hallLength), Quaternion.identity, roomTransform)
                        .GetComponent<Door>();
                    prevRoomDoor.myRoom = pRoom;
                    thisRoomDoor.myRoom = this;
                    
                    //room spawning
                    for (int x = 0; x < width; x++) {
                        for (int y = 0; y < height; y++) {
                            roomData.tMap.SetTile(new Vector3Int(doorPos.x + hallLength - 1 + x, doorPos.y - height/2 + y, 0), roomData.labTiles.roomRule);
                        }
                    }
                    
                    //hallway spawning
                    for (int i = 0; i < hallLength; i++) {
                        roomData.tMap.SetTile(new Vector3Int(doorPos.x + i, doorPos.y , 0), roomData.labTiles.horizontalHallRule);
                        roomData.tMap.SetTile(new Vector3Int(doorPos.x + i, doorPos.y-1, 0), roomData.labTiles.horizontalHallRule);
                    }
                    var miniMapHallwayR = Object.Instantiate(roomData.miniMapHallwayPrefab, new Vector3(doorPos.x + hallLength/2, doorPos.y, 0),
                        Quaternion.identity, roomData.miniMap.transform);
                    miniMapHallwayR.transform.localScale = new Vector3(hallLength, 2, 1);
                    miniMapRoom.hallwaySR = miniMapHallwayR.GetComponent<SpriteRenderer>();
                    

                    break;

                case Direction.Down:
                    //door spawning
                    prevRoomDoor = Object.Instantiate(roomData.labTiles.dDoor, (Vector3)doorPos + Vector3.up*dDoorOffset, Quaternion.identity, pRoom.roomGameObject.transform)
                        .GetComponent<Door>();
                    thisRoomDoor = Object.Instantiate(roomData.labTiles.uDoor, (Vector3)doorPos + Vector3.down * (hallLength-1), Quaternion.identity, roomTransform)
                        .GetComponent<Door>();
                    prevRoomDoor.myRoom = pRoom;
                    thisRoomDoor.myRoom = this;
                    
                    //room spawning
                    for (int x = 0; x < width; x++) {
                        for (int y = 0; y < height; y++) {
                            roomData.tMap.SetTile(new Vector3Int(doorPos.x - width / 2 + x, doorPos.y - hallLength + 1 - y, 0), roomData.labTiles.roomRule);
                        }
                    }
                    
                    //hallway spawning
                    for (int i = 0; i < hallLength; i++) {
                        roomData.tMap.SetTile(new Vector3Int(doorPos.x, doorPos.y - i, 0), roomData.labTiles.verticalHallRule);
                        roomData.tMap.SetTile(new Vector3Int(doorPos.x-1, doorPos.y - i, 0), roomData.labTiles.verticalHallRule);
                    }
                    var miniMapHallwayD = Object.Instantiate(roomData.miniMapHallwayPrefab, new Vector3(doorPos.x, doorPos.y - hallLength/2, 0),
                        Quaternion.identity, roomData.miniMap.transform);
                    miniMapHallwayD.transform.localScale = new Vector3(2, hallLength, 1);
                    miniMapRoom.hallwaySR = miniMapHallwayD.GetComponent<SpriteRenderer>();
                    
                    break;

                case Direction.Left:
                    //door spawning
                    prevRoomDoor = Object.Instantiate(roomData.labTiles.lDoor, (Vector3)doorPos + Vector3.right, Quaternion.identity, pRoom.roomGameObject.transform)
                        .GetComponent<Door>();
                    thisRoomDoor = Object.Instantiate(roomData.labTiles.rDoor, (Vector3)doorPos + Vector3.left * (hallLength-1), Quaternion.identity, roomTransform)
                        .GetComponent<Door>();
                    prevRoomDoor.myRoom = pRoom;
                    thisRoomDoor.myRoom = this;

                    //room spawning
                    for (int x = 0; x < width; x++) {
                        for (int y = 0; y < height; y++) {
                            roomData.tMap.SetTile(new Vector3Int(doorPos.x - hallLength + 1 - x, doorPos.y - height / 2 + y, 0), roomData.labTiles.roomRule);
                        }
                    }
                    
                    //hallway spawning
                    for (int i = 0; i < hallLength; i++) {
                        roomData.tMap.SetTile(new Vector3Int(doorPos.x - i, doorPos.y, 0), roomData.labTiles.horizontalHallRule);
                        roomData.tMap.SetTile(new Vector3Int(doorPos.x - i, doorPos.y-1, 0), roomData.labTiles.horizontalHallRule);
                    }
                    var miniMapHallwayL = Object.Instantiate(roomData.miniMapHallwayPrefab, new Vector3(doorPos.x - hallLength/2, doorPos.y, 0),
                        Quaternion.identity, roomData.miniMap.transform);
                    miniMapHallwayL.transform.localScale = new Vector3(hallLength, 2, 1);
                    miniMapRoom.hallwaySR = miniMapHallwayL.GetComponent<SpriteRenderer>();

                    break;
                
                default:
                    Debug.LogError("INVALID DIRECTION");
                    return null;
            }
            DrawWalls(roomData.tMap, roomData.labTiles);
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

        private async void SetConnectedRooms(RoomData roomData, Direction dir)
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
                    new Room(roomData, potentialRooms[connectionDir], connectionDir, this));
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

        private FloorGenerator.RoomCategory.RoomWeight PickRoomType(FloorGenerator.RoomCategory[] roomCategories, 
            int numRooms, out FloorGenerator.RoomCategory category)
        {
            var randValue = Random.Range(0, 1f);
            if (numRooms > 1) 
            {
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
                if (category.categoryName == FloorGenerator.CategoryName.End)
                    category = roomCategories.First(rc => rc.amountToCreate >= 1);
            }
            //if there is only one room left to create make it the exit room
            else
                category = roomCategories.First(rc => rc.categoryName == FloorGenerator.CategoryName.End);
            randValue = Random.Range(0, category.totalProb);
            var returnRoom = category.roomTypes[0]; //Sometimes causes out of range
            foreach (var rt in category.roomTypes)
            {
                if (rt.weight >= randValue) {
                    return rt;
                }
                randValue -= rt.weight;
            }
            return returnRoom;
        }
    }
}