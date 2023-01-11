using UnityEngine;

namespace EnemyScripts
{
    /// <summary>
    /// interface identifying a class that can shoot projectiles 
    /// </summary>
    public interface IFire
    {
        void Fire();
        
        bool CanShoot { set; }
    }
}