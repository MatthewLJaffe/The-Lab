using General;
using UnityEngine;

namespace EntityStatsScripts
{
    public class DamageNumber : MonoBehaviour, IPooled
    {
        public GameObjectPool MyPool { get; set; }
    
        public void ReturnToMyPool()
        {
            MyPool.ReturnToPool(gameObject);
        }
    }
}
