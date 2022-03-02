using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LabCreationScripts
{
    public class RoomData
    {
        public Tilemap tMap;
        public LabTiles labTiles;
        public RoomDimensions dimensions;
        public Room[] rooms;
        public FloorGenerator.RoomCategory[] roomCategories;
        public Transform parent;
        public GameObject roomPrefab;
        public GameObject miniMap;
        public GameObject miniMapRoomPrefab;
        public GameObject miniMapHallwayPrefab;
        public Action finish;

        public RoomData(Tilemap tMap, LabTiles labTiles,
            RoomDimensions dimensions, Room[] rooms, FloorGenerator.RoomCategory[] roomCategories,
            Transform parent, GameObject miniMap, GameObject roomPrefab, GameObject miniMapRoomPrefab, GameObject miniMapHallwayPrefab,
            Action finish)
        {
            this.tMap = tMap;
            this.labTiles = labTiles;
            this.dimensions = dimensions;
            this.rooms = rooms;
            this.roomCategories = roomCategories;
            this.parent = parent;
            this.miniMap = miniMap;
            this.roomPrefab = roomPrefab;
            this.miniMapRoomPrefab = miniMapRoomPrefab;
            this.miniMapHallwayPrefab = miniMapHallwayPrefab;
            this.finish = finish;
        }
    }

    [System.Serializable]
    public struct RoomDimensions
    {
        public int minHallway;
        public int maxHallway;
    }
    
    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }

    [System.Serializable]
    public struct LabTiles 
    {
        public RuleTile roomRule;
        public RuleTile wallRule;
        public RuleTile verticalHallRule;
        public RuleTile horizontalHallRule;
        public GameObject uDoor;
        public GameObject dDoor;
        public GameObject rDoor;
        public GameObject lDoor;
    }
}