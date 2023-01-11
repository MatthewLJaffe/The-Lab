using General;
using UnityEngine;

namespace EntityStatsScripts
{
    /// <summary>
    /// component that pools damage number text when entity takes damage
    /// </summary>
    public class DamageNumber : MonoBehaviour, IPooled
    {
        public GameObjectPool MyPool { get; set; }
    
        public void ReturnToMyPool()
        {
            MyPool.ReturnToPool(gameObject);
        }
    }
}
