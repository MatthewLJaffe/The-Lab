using PlayerScripts;
using UnityEngine;

namespace CameraScripts
{
    /// <summary>
    /// component for invoking screen shake
    /// </summary>
    public class ShakeScreen : MonoBehaviour
    {
        [SerializeField] private float defaultShake;

        public void Shake(float amount)
        {
            CameraShakeController.invokeShake(amount);
        }

        public void Shake()
        {
            Shake(defaultShake);
        }
    }
}