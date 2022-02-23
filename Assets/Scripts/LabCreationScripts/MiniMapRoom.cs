using System;
using UnityEngine;

namespace LabCreationScripts
{
    public class MiniMapRoom : MonoBehaviour
    {
        [SerializeField] private Color hideColor;
        [SerializeField] private Color revealColor;
        [SerializeField] private SpriteRenderer roomSR;
        public SpriteRenderer hallwaySR;
        public Room myRoom;

        private void Awake()
        {
            Door.onEnterRoom += RevealRoom;
            roomSR.color = hideColor;
            if (hallwaySR != null)
                hallwaySR.color = hideColor;
        }

        private void OnDestroy()
        {
            Door.onEnterRoom -= RevealRoom;
        }

        public void RevealRoom()
        {
            roomSR.color = revealColor;
            foreach (var room in myRoom.ConnectedRooms.Values)
            {
                if (room.miniMapRoom)
                    room.miniMapRoom.hallwaySR.color = revealColor;
            }
        }


        private void RevealRoom(Room roomDiscovered)
        {
            if (roomDiscovered != myRoom) return;
            roomSR.color = revealColor;
            foreach (var room in myRoom.ConnectedRooms.Values)
            {
                if (room.miniMapRoom)
                    room.miniMapRoom.hallwaySR.color = revealColor;
            }
                    
        }

        
    }
}