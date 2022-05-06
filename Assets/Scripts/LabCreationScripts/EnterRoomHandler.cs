using System;
using UnityEngine;
using UnityEngine.Events;

namespace LabCreationScripts
{
    public class EnterRoomHandler : MonoBehaviour
    {
        [SerializeField] private Transform roomChild;
        private Room _myRoom;
        private bool _inMyRoom;
        public UnityEvent enterRoom;
        public UnityEvent exitRoom;

        private void Awake()
        {
            Door.onEnterRoom += CheckEnterRoom;
        }
        
        private void Start()
        {
            _myRoom = roomChild.parent.GetComponent<RoomInstance>().myRoom;
        }

        private void CheckEnterRoom(Room currRoom)
        {
            if (currRoom == _myRoom)
            {
                enterRoom.Invoke();
                _inMyRoom = true;
            }
            else if (_inMyRoom)
            {
                enterRoom.Invoke();
                _inMyRoom = false;
            }
        }
    }
}