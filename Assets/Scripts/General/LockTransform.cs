using System;
using UnityEngine;

namespace General
{
    public class LockTransform : MonoBehaviour
    {
        private void LateUpdate()
        {
            transform.rotation = Quaternion.identity;
        }
    }
}