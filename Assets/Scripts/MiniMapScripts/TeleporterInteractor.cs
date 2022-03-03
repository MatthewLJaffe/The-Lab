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
            _myRoom = GetComponentInParent<RoomInstance>().myRoom;
            EnemyHandler.onRoomClear += EnableTeleporter;
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