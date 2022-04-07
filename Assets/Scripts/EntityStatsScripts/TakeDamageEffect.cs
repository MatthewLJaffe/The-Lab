using System;
using System.Collections;
using EnemyScripts;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace EntityStatsScripts
{
    public class TakeDamageEffect : MonoBehaviour
    {
        [SerializeField] private UnityEvent onPlayDeath;
        [SerializeField] private SpriteRenderer mainSr;
        [SerializeField] private AnimationCurve flashCurve;
        [SerializeField] private AnimationCurve deathFlashCurve;
        [SerializeField] private AnimationCurve deathScaleCurve;
        [SerializeField] private float flashTime;
        [SerializeField] private float deathTime;
        [SerializeField] private Enemy enemy;
        private SpriteRenderer _flashSr;
        private Coroutine _flashRoutine;
        private KnockBack _knockBack;
        private bool _synchronize;

        private void Awake()
        {
            _flashSr = GetComponent<SpriteRenderer>();
            enemy.enemyKilled += DeathFlash;
            _knockBack = enemy.GetComponentInChildren<KnockBack>();
        }

        private void OnDestroy()
        {
            enemy.enemyKilled -= DeathFlash;
        }

        private void Update()
        {
            if (_synchronize)
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
            _synchronize = true;
            var transparent = new Color(1f, 1f, 1f, 0f);
            for (float currTime = 0; currTime <= flashTime; currTime += Time.deltaTime)
            {
                transparent.a = flashCurve.Evaluate(currTime / flashTime);
                _flashSr.color = transparent;
                yield return null;
            }
            _flashSr.color = new Color(1f,1f,1f,0);
            _synchronize = false;
            _flashRoutine = null;
        }

        private IEnumerator PlayDeathEffect()
        {
            onPlayDeath.Invoke();
            var transparent = new Color(1f, 1f, 1f, 0f);
            for (float t = 0; t <= deathTime; t += Time.deltaTime)
            {
                transparent.a = deathFlashCurve.Evaluate(t / deathTime);
                var currSize = deathScaleCurve.Evaluate(t / deathTime);
                var scale = new Vector3(currSize, currSize, 1f);
                enemy.transform.localScale = scale;
                _flashSr.color = transparent;
                if (_flashSr.color.a > .99f) {
                    mainSr.color = new Color(1f, 1f, 1f, 0f);
                }
                yield return null;
            }
            Destroy(enemy.gameObject);
        }

        public void RotateWithKnockBack(GameObject rotate)
        {
            if (!_knockBack) return;
            rotate.transform.right = _knockBack.knockBackDir;
        }

        public void DeathFlash()
        {
            _synchronize = true;
            StartCoroutine(PlayDeathEffect());
        }
    }
}