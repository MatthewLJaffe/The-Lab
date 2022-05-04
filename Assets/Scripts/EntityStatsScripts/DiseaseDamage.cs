using System;
using System.Collections;
using System.Collections.Generic;
using EntityStatsScripts;
using UnityEngine;
using Random = UnityEngine.Random;

public class DiseaseDamage : MonoBehaviour
{
    [SerializeField] private Vector2 timeRangeBetweenBursts;
    [SerializeField] private Vector2 burstTimeRange;
    [SerializeField] private float dps;
    [SerializeField] private Collider2D damageCollider;
    private Coroutine _damageRoutine;
    private ParticleSystem _particleSystem;

    private void Awake()
    {
        _particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    private void Start()
    {
        StartCoroutine(CooldownRoutine());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
            _damageRoutine = StartCoroutine(DamageRoutine());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_damageRoutine != null)
            StopCoroutine(_damageRoutine);
        _damageRoutine = null;
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
        damageCollider.enabled = true;
        _particleSystem.Play();
        yield return new WaitForSeconds(Random.Range(burstTimeRange.x, burstTimeRange.y));
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator DamageRoutine()
    {
        var waitForFixed = new WaitForFixedUpdate();
        while (true)
        {
            yield return waitForFixed;
            PlayerBarsManager.Instance.ModifyPlayerStat(PlayerBar.PlayerBarType.Infection, -dps * Time.fixedDeltaTime);
        }
    }
}
