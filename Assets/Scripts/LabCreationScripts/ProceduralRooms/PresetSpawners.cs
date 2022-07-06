using UnityEngine;

namespace LabCreationScripts.ProceduralRooms
{
    [CreateAssetMenu(fileName = "PresetSpawners")]
    public class PresetSpawners : ScriptableObject
    {
        public ProceduralRoom.SpawnData[] spawners;
    }
}