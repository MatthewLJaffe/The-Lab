using System;
using System.Collections;
using UnityEngine;

namespace EntityStatsScripts
{
    public class TakeDamageFlash : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer mainSr;
        [SerializeField] private AnimationCurve flashCurve;
        [SerializeField] private float flashTime;
        private SpriteRenderer _flashSr;

        private void Awake()
        {
            _flashSr = GetComponent<SpriteRenderer>();
        }

        private void FixedUpdate()
        {
            _flashSr.sprite = mainSr.sprite;
        }

        private void OnEnable()
        {
            StartCoroutine(PlayFlash());
        }

        private IEnumerator PlayFlash()
        {
            var transparent = new Color(1f, 1f, 1f, 1f);
            for (float currTime = 0; currTime <= flashTime; currTime += Time.deltaTime)
            {
                transparent.a = 1f - flashCurve.Evaluate(currTime / flashTime);
                mainSr.color = transparent;
                yield return null;
            }
            mainSr.color = Color.white;
            gameObject.SetActive(false);
        }
    }
}