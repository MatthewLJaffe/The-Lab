using System;
using EnemyScripts;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace LabCreationScripts.ProceduralRooms
{
    [CreateAssetMenu(fileName = "EnemyRoom", menuName = "ProceduralRooms/EnemyRoom")]
    public class EnemyRoom : ProceduralRoom
    {
        [SerializeField] protected GameObject enemyHandlerPrefab;
        protected EnemyHandler enemyHandler;
        
        [Serializable]
        public struct EnemyWeight
        {
            public GameObject enemy;
            public float weight;

        }
        public override void FillRoom(Room room, Tilemap tmap, GameObject roomGameObject)
        {
            base.FillRoom(room, tmap, roomGameObject);
            var enemyHandlerGO = Instantiate(enemyHandlerPrefab, Vector3.zero, Quaternion.identity,
                roomGameObject.transform);
            enemyHandler = enemyHandlerGO.GetComponent<EnemyHandler>();
            enemyHandler.myRoom = room;
        }
    }
}