using System;
using System.Collections;
using EnemyScripts;
using General;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EntityStatsScripts
{
    public class EntityHealth : MonoBehaviour, IDamageable
    {
        public Action onTakeDamage = delegate {  };
        [SerializeField] protected float maxHealth;
        [Tooltip("Should be uninitialized for player")]
        [SerializeField] protected Slider slider;
        [SerializeField] protected GameObject numberPrefab;
        [SerializeField] protected Transform displayPoint;
        [SerializeField] private Animator animator;
        private TakeDamageFlash _takeDamageFlash;
        private float _currentHealth;
        private float _displayAmount;
        private GameObjectPool _damageNumberPool;
        private Coroutine _currentDisplay;
        private KnockBack _knockBack;

        protected virtual void Awake()
        {
            _currentHealth = maxHealth;
            _damageNumberPool = new GameObjectPool(numberPrefab, displayPoint);
            _knockBack = GetComponent<KnockBack>();
            _takeDamageFlash = GetComponentInChildren<TakeDamageFlash>(true);
            if (slider != null)
            {
                slider.maxValue = maxHealth;
                slider.value = maxHealth;
            }
        }
        
        public virtual void TakeDamage(float amount, Vector2 dir)
        {
            if (Mathf.Abs(amount) < .1f)
                return;
            onTakeDamage.Invoke();
            amount = Mathf.Round(amount);
            _displayAmount += amount;
            _currentHealth -= amount;
            slider.value = _currentHealth;
            TakeDamageFlash();
            if (_currentDisplay == null)
            {
                var number = _damageNumberPool.GetFromPool();
                _currentDisplay = StartCoroutine(DamageBuffer(dir, number.GetComponent<TextMeshProUGUI>()));
            }
            if (_currentHealth <= 0)
                Die();
        }

        protected void TakeDamageFlash()
        {
            if (_takeDamageFlash) {
                _takeDamageFlash.Flash();
            }
        }

        protected virtual void Die()
        {
            
            animator.SetBool("Die", true);
        }

        protected virtual void DestroyEntity()
        {
            Destroy(gameObject);
        }

        private IEnumerator DamageBuffer(Vector2 dir, TextMeshProUGUI tmNumber)
        {
            yield return new WaitForSeconds(.1f);
            tmNumber.text = "" + _displayAmount;
            if (_knockBack)
                _knockBack.ApplyKnockBack(_displayAmount, dir);
            _displayAmount = 0;
            _currentDisplay = null;
        }
    }
    
}
