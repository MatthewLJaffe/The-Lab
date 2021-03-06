using System;
using EnemyScripts;
using LabCreationScripts;
using PlayerScripts;
using UnityEngine;

namespace MiniMapScripts
{
    public class TeleporterInteractor : MonoBehaviour, IInteractable
    {
        public static Action<bool> teleportInteract = delegate { };
        [SerializeField] private Color interactColor;
        [SerializeField] private SpriteRenderer sr;
        [SerializeField] private GameObject teleporterUi;
        private static bool _teleportEnabled;
        [SerializeField] private bool roomClear;
        private Room _myRoom;

        private void Awake()
        {
            var roomInstance = GetComponentInParent<RoomInstance>();
            if (roomInstance != null) {
                _myRoom = roomInstance.myRoom;
                if (_myRoom.roomType.lockRoom) {
                    EnemyHandler.onRoomClear += EnableTeleporter;
                }
                else {
                    Door.onEnterRoom += EnableTeleporter;
                }
            }
            //beginning teleporter
            else
            {
                roomClear = true;
                teleporterUi.SetActive(true);
            }
        }

        private void OnDestroy()
        {
            EnemyHandler.onRoomClear -= EnableTeleporter;
            Door.onEnterRoom -= EnableTeleporter;
        }

        private void EnableTeleporter(Room room)
        {
            if (_myRoom != room) return;
            roomClear = true;
            teleporterUi.SetActive(true);
        }
        

        public bool CanInteract
        {
            set => sr.color = value && roomClear ? interactColor : Color.white;
        }

        public void Interact()
        {
            if (!roomClear) return;
            _teleportEnabled = !_teleportEnabled;
            teleportInteract.Invoke(_teleportEnabled);
        }
    }
}