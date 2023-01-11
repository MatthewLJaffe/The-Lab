using System;
using System.Collections;
using General;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace EntityStatsScripts
{
    /// <summary>
    /// applies disease damage to player when entering hitbox used on toxic vents
    /// </summary>
    public class DiseaseDamage : MonoBehaviour
    {
        [SerializeField] private Vector2 startDelayTimeRange;
        [SerializeField] private Vector2 timeRangeBetweenBursts;
        [SerializeField] private Vector2 burstTimeRange;
        [SerializeField] private float dps;
        [SerializeField] private Collider2D damageCollider;
        [SerializeField] private bool startOnAwake = true;
        public UnityEvent onGasStart;
        private Coroutine _damageRoutine;
        private ParticleSystem _particleSystem;
        private bool _active;
        private static DiseaseDamage _damagingPlayer;

        private void Awake()
        {
            _particleSystem = GetComponentInChildren<ParticleSystem>();
        }

        private void Start()
        {
            if (startOnAwake)
                StartCoroutine(BeginRoutine());        
        }

        public void StartVent()
        {
            StartCoroutine(BeginRoutine());
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player") && !_damagingPlayer)
            {
                _damagingPlayer = this;
                _damageRoutine = StartCoroutine(DamageRoutine());
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player") && _damagingPlayer == this)
            {
                StopCoroutine(_damageRoutine);
                _damagingPlayer = null;
            }
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
