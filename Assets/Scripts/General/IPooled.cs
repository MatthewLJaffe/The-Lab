using System;
using System.Collections.Generic;
using UnityEngine;

namespace General
{
    /// <summary>
    /// allows object to return to pool 
    /// </summary>
    public interface IPooled
    {
        GameObjectPool MyPool { get; set; }
        void ReturnToMyPool();
    }
}