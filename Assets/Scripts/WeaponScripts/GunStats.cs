using UnityEngine;

namespace WeaponScripts
{
    [CreateAssetMenu(fileName = "GunStats")]
    public class GunStats : ScriptableObject
    {
        public float damage;
        public float fireRate;
        public float accuracy;
        public float critChance;
        public float reloadSpeed;
        public int magSize;
    }
}
