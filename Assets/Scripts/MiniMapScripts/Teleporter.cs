using System;
using PlayerScripts;
using UnityEngine;

namespace MiniMapScripts
{
    public class Teleporter : MonoBehaviour
    {
        public Transform teleportLocation;
        private GameObject _player;

        private void Start()
        {
            _player = PlayerFind.instance.playerInstance;
        }

        public void Teleport()
        {
            _player.transform.position = teleportLocation.position;
        }
    }
}