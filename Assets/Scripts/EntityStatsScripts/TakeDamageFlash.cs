using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace EntityStatsScripts
{
    public class TakeDamageFlash : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer mainSr;
        [SerializeField] private AnimationCurve flashCurve;
        [SerializeField] private AnimationCurve deathCurve;
        [SerializeField] private float flashTime;
        [SerializeField] private float deathTime;
        private SpriteRenderer _flashSr;
        private Coroutine _flashRoutine;
        private bool _syncronize;

        private void Awake()
        {
            _flashSr = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (_syncronize)
            {
                _flashSr.sprite = mainSr.sprite;
                _flashSr.flipX = mainSr.flipX;
            }
        }

        public void Flash()
        {
            if (_flashRoutine == null)
                _flashRoutine = StartCoroutine(PlayFlash());
        }

        private IEnumerator PlayFlash()
        {
            _syncronize = true;
            var transparent = new Color(1f, 1f, 1f, 0f);
            for (float currTime = 0; currTime <= flashTime; currTime += Time.deltaTime)
            {
                transparent.a = flashCurve.Evaluate(currTime / flashTime);
                _flashSr.color = transparent;
                yield return null;
            }
            _flashSr.color = new Color(1f,1f,1f,0);
            _syncronize = false;
        }

        private IEnumerator PlayDeathFlash()
        {
            var transparent = new Color(1f, 1f, 1f, 0f);
            for (float t = 0; t <= deathTime; t += Time.deltaTime)
            {
                transparent.a = deathCurve.Evaluate(t);
                _flashSr.color = transparent;
                yield return null;
            }

            _flashRoutine = null;
        }

        public void DeathFlash()
        {
            _syncronize = true;
            StartCoroutine(PlayDeathFlash());
        }
    }
}