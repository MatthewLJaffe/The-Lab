using UnityEngine;

namespace LabCreationScripts
{
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