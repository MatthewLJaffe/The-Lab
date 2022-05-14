using System.Collections;
using General;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace EntityStatsScripts
{
    public class DiseaseDamage : MonoBehaviour
    {
        [SerializeField] private Vector2 startDelayTimeRange;
        [SerializeField] private Vector2 timeRangeBetweenBursts;
        [SerializeField] private Vector2 burstTimeRange;
        [SerializeField] private float dps;
        [SerializeField] private Collider2D damageCollider;
        public UnityEvent onGasStart;
        private Coroutine _damageRoutine;
        private ParticleSystem _particleSystem;
        private static bool _damagingPlayer;

        private void Awake()
        {
            _particleSystem = GetComponentInChildren<ParticleSystem>();
        }

        private void Start()
        {
            StartCoroutine(BeginRoutine());        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player") && !_damagingPlayer)
            {
                _damagingPlayer = true;
                StartCoroutine(DamageRoutine());
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player") && _damagingPlayer)
                _damagingPlayer = false;
        }

        private IEnumerator BeginRoutine()
        {
            yield return new WaitForSeconds(Random.Range(startDelayTimeRange.x, startDelayTimeRange.y));
            StartCoroutine(ActiveRoutine());
        }

        private IEnumerator CooldownRoutine()
        {
            _particleSystem.Stop();
            damageCollider.enabled = false;
            yield return new WaitForSeconds(Random.Range(timeRangeBetweenBursts.x, timeRangeBetweenBursts.y));
            StartCoroutine(ActiveRoutine());
        }

        private IEnumerator ActiveRoutine()
        {
            onGasStart.Invoke();
            damageCollider.enabled = true;
            _particleSystem.Play();
            yield return new WaitForSeconds(Random.Range(burstTimeRange.x, burstTimeRange.y));
            StartCoroutine(CooldownRoutine());
        }

        private IEnumerator DamageRoutine()
        {
            var waitForFixed = new WaitForFixedUpdate();
            while (_damagingPlayer)
            {
                yield return waitForFixed;
                PlayerBarsManager.Instance.ModifyPlayerStat(PlayerBar.PlayerBarType.Infection, -dps * Time.fixedDeltaTime);
            }
        }
    }
}
