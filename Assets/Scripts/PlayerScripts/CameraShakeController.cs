using System;
using UnityEngine;
using System.Threading.Tasks;
using Random = UnityEngine.Random;


namespace PlayerScripts
{
    public class CameraShakeController : MonoBehaviour
    {
        public static Action<float> InvokeShake = delegate { };
        [SerializeField] private float shakeTime;
        [SerializeField] private GameObject cam;
        [SerializeField] private float minShake;
        [SerializeField] private float shakeFactor;

        private void Awake()
        {
            InvokeShake += ShakeScreen;
        }

        private async void ShakeScreen(float intensity)
        {
            var shakeDisp = intensity * shakeFactor;
            Vector2 shakeDir = new Vector2(Random.Range(0, 1f), Random.Range(0, 1f)) * shakeDisp;
            while (shakeDisp > minShake) {
                await MoveToAndBack(shakeDir, shakeTime);
                shakeDir *
            }
        }

        private async Task MoveToAndBack(Vector2 dir, float time)
        {
            for (float t = 0; t < time; t += Time.fixedDeltaTime)
            {
                cam.transform.position += (Vector3) dir * Time.fixedDeltaTime / time;
                await Task.Delay((int)Time.fixedDeltaTime * 1000);
            }
        }
    }
}