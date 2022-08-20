using UnityEngine;

namespace EnemyScripts
{
    public interface IFire
    {
        void Fire();
        
        bool CanShoot { set; }
    }
}