using System;
using System.Collections.Generic;
using UnityEngine;

namespace General
{
    public interface IPooled
    {
        GameObjectPool MyPool { get; set; }
        void ReturnToMyPool();
    }
}