using System;
using EnemyScripts;
using UnityEngine;

namespace LabCreationScripts
{
    public class Door : MonoBehaviour
    {
        public static Action<Room> onEnterRoom = delegate {  };
        private bool _enteredPreviously;
        public Direction doorDir;
        private BoxCollider2D _doorCollider;
        private CapsuleCollider2D _doorTrigger;
        public Room myRoom;
        private static bool _firstRoomEntered;
        private bool _doorOpen;
        private DoorAnimator _doorAnimator;

        private void Awake()
        {
            onEnterRoom += CloseDoor;
            onEnterRoom += SetDoorTrigger;
            EnemyHandler.onRoomClear += UnlockDoor;
            FloorGenerator.onFloorFinished += EnteredFirstRoom;
            _doorCollider = GetComponent<BoxCollider2D>();
            _doorTrigger = GetComponent<CapsuleCollider2D>();
            _doorAnimator = GetComponentInChildren<DoorAnimator>();
            UnlockDoor();
        }

        private void EnteredFirstRoom()
        {
            if (!_firstRoomEntered && myRoom.RoomId == 0)
            {
                onEnterRoom.Invoke(myRoom);
                _firstRoomEntered = true;
            }
        }

        private void CloseDoor(Room room)
        {
            if (room != myRoom || _enteredPreviously || myRoom.RoomId == 0) return;
            _enteredPreviously = true;
            _doorCollider.enabled = true;
            _doorAnimator.ShowDoorClosed();
        }
        
        private void SetDoorTrigger(Room room)
        {
            _doorTrigger.enabled = room.RoomId != myRoom.RoomId;
        }

        private void UnlockDoor(Room room = null)
        {
            if (room == null || room == myRoom && _doorCollider)
                _doorCollider.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                onEnterRoom(myRoom);
                Debug.Log("Entered room " + myRoom.RoomId);
            }
        }

        private void OnDestroy()
        {
            onEnterRoom -= CloseDoor;
            onEnterRoom -= SetDoorTrigger;
            EnemyHandler.onRoomClear -= UnlockDoor;
            FloorGenerator.onFloorFinished -= EnteredFirstRoom;


        }
    }
}
