using System;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;


namespace CameraScripts
{
    /// <summary>
    /// used by other classes to create screenshake via static refrence
    /// </summary>
    public class CameraShakeController : MonoBehaviour
    {
        public static Action<float> invokeShake = delegate { };
        [SerializeField] private float shakeTime;
        [SerializeField] private GameObject cam;
        [SerializeField] private float minShake;
        [SerializeField] private float shakeFactor;
        [SerializeField] private float randomComponent;
        [SerializeField] private float shakeStep;
        private float _shakeDisp;
        private bool _shaking;

        private void Awake()
        {
            invokeShake += ShakeScreen;
        }

        private void OnDestroy()
        {
            invokeShake -= ShakeScreen;
        }

        //shakes camera in semirandom directions with displacement decreasing after each shake
        private async void ShakeScreen(float intensity)
        {
            if (!cam) return;
            _shakeDisp = Mathf.Max(intensity * shakeFactor, _shakeDisp);
            if (_shaking) return;
            _shaking = true;
            var initialPos = cam.transform.localPosition;
            Vector2 shakeDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * _shakeDisp;
            while (_shakeDisp > minShake)
            {
                await Move(shakeDir, shakeTime);
                _shakeDisp *= shakeStep;
                //make new direction opposite and smaller magnitude
                shakeDir = -shakeDir + _shakeDisp * (1f - randomComponent) * -shakeDir;
                shakeDir += new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * (randomComponent * _shakeDisp);
            }
            await Move(initialPos - cam.transform.localPosition, shakeTime);
            _shaking = false;
        }

        //asyc move camera to pos in time
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