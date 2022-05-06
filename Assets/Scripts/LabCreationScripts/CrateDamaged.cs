using System;
using EntityStatsScripts;
using UnityEngine;
using UnityEngine.Events;

namespace LabCreationScripts
{
    public class CrateDamaged : MonoBehaviour, IDamageable
    {
        [SerializeField] private Sprite[] crateSprites;
        private int _timesDamaged;
        private SpriteRenderer _sr;
        public UnityEvent onDestroyed;

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
        }

        public void TakeDamage(float amount, Vector2 dir)
        {
            _timesDamaged++;
            if (_timesDamaged < crateSprites.Length) {
                _sr.sprite = crateSprites[_timesDamaged];
            }
            if (_timesDamaged == crateSprites.Length -1)
                onDestroyed.Invoke();
        }
    }
}