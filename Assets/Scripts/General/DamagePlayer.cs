using System;
using General;
using UnityEngine;

namespace PlayerScripts
{
    public static class DamagePlayer
    {
        public static Action<float, Vector2, DamageSource, bool> applyPlayerDamage = delegate {  };
    }
}