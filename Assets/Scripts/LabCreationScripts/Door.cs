using System;
using EnemyScripts;
using General;
using UnityEngine;

namespace LabCreationScripts
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private SoundEffect closeDoorSound;
        public static Action<Room> onEnterRoom = delegate {  };
        private bool _enteredPreviously;
        public Direction doorDir;
        private BoxCollider2D _doorCollider;
        private CapsuleCollider2D _doorTrigger;
        public Room myRoom;
        private bool _doorOpen;
        private DoorAnimator _doorAnimator;
        public bool lockable = true;

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
            if (myRoom.RoomId == 0)
            {
                onEnterRoom.Invoke(myRoom);
            }
        }

        public void CloseDoor()
        {
            if (lockable)
                _doorCollider.enabled = true;
            _doorAnimator.ShowDoorClosed();
            closeDoorSound.Play(gameObject);
        }
        
        private void CloseDoor(Room room)
        {
            if (myRoom == null || room != myRoom || _enteredPreviously || myRoom.RoomId == 0) return;
            _enteredPreviously = true;
            if (lockable)
                _doorCollider.enabled = true;
            _doorAnimator.ShowDoorClosed();
            closeDoorSound.Play(gameObject);
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
                if (!lockable)
                    EnemyHandler.onRoomClear(myRoom);
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
