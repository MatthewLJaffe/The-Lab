using System;
using UnityEngine;
using System.Threading.Tasks;
using Random = UnityEngine.Random;


namespace PlayerScripts
{
    public class CameraShakeController : MonoBehaviour
    {
        public static Action<float> invokeShake = delegate { };
        [SerializeField] private float shakeTime;
        [SerializeField] private GameObject cam;
        [SerializeField] private float minShake;
        [SerializeField] private float shakeFactor;
        [SerializeField] private float randomComponent;
        [SerializeField] private float shakeStep;

        private void Awake()
        {
            invokeShake += ShakeScreen;
        }

        private void OnDestroy()
        {
            invokeShake -= ShakeScreen;
        }

        private async void ShakeScreen(float intensity)
        {
            var initialPos = cam.transform.localPosition;
            var shakeDisp = intensity * shakeFactor;
            Vector2 shakeDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * shakeDisp;
            while (shakeDisp > minShake)
            {
                await Move(shakeDir, shakeTime);
                shakeDisp *= shakeStep;
                //make new direction opposite and smaller magnitude
                shakeDir = -shakeDir + -shakeDisp * shakeDir;
                shakeDir += new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * (randomComponent * shakeDisp);
            }
            await Move(initialPos - cam.transform.localPosition, shakeTime);
        }

        private async Task Move(Vector2 dir, float time)
        {
            var dest = cam.transform.localPosition + (Vector3)dir;
            for (float t = 0; t <= time; t += Time.fixedDeltaTime)
            {
                cam.transform.position += (Vector3) dir * Time.fixedDeltaTime / time;
                await Task.Delay((int)(Time.fixedDeltaTime * 1000));
            }
            cam.transform.localPosition = dest;
        }
    }
}