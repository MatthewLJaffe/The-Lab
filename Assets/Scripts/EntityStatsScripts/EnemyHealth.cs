using System;
using System.Collections;
using EnemyScripts;
using General;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EntityStatsScripts
{
    public class EnemyHealth : MonoBehaviour, IDamageable
    {
        public static Action<Vector3> killedByHazard = delegate {  };
        public Action onTakeDamage = delegate {  };
        [SerializeField] protected float maxHealth;
        [SerializeField] protected Slider slider;
        [SerializeField] protected GameObject numberPrefab;
        [SerializeField] protected Transform displayPoint;
        [SerializeField] private Color critColor;
        private Enemy _enemy;
        private TakeDamageEffect _takeDamageEffect;
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
            _takeDamageEffect = GetComponentInChildren<TakeDamageEffect>(true);
            _enemy = GetComponent<Enemy>();
            if (slider != null)
            {
                slider.maxValue = maxHealth;
                slider.value = maxHealth;
            }
        }

        public void ScaleHealth(float scalar)
        {
            maxHealth *= scalar;
            if (slider != null)
            {
                slider.maxValue = maxHealth;
                slider.value = maxHealth;
            }
        }
        
        public virtual void TakeDamage(float amount, Vector2 dir, DamageSource source, bool crit = false)
        {
            if (Mathf.Abs(amount) < .1f)
                return;
            onTakeDamage.Invoke();
            amount = Mathf.Round(amount);
            _displayAmount += amount;
            _currentHealth -= amount;
            slider.value = _currentHealth;
            if (_currentDisplay == null)
            {
                var number = _damageNumberPool.GetFromPool();
                _currentDisplay = StartCoroutine(DamageBuffer(dir, number.GetComponent<TextMeshProUGUI>(), crit));
            }
            if (_currentHealth > 0)
                TakeDamageFlash();
            else
                Die(source.isHazard);

        }

        protected void TakeDamageFlash()
        {
            if (_takeDamageEffect) {
                _takeDamageEffect.Flash();
            }
        }

        protected virtual void Die (bool hazardKill)
        {
            if (hazardKill)
                killedByHazard.Invoke(transform.position);
            _enemy.Death();
        }

        protected virtual void DestroyEntity()
        {
            Destroy(gameObject);
        }

        private IEnumerator DamageBuffer(Vector2 dir, TextMeshProUGUI tmNumber, bool crit)
        {
            //set immediately so it can be used by other effect scripts
            _knockBack.knockBackDir = dir.normalized;
            yield return new WaitForSeconds(.1f);
            tmNumber.text = "" + _displayAmount;
            tmNumber.color = crit ? critColor : Color.white;
            if (_knockBack)
                _knockBack.ApplyKnockBack(_displayAmount, dir);
            _displayAmount = 0;
            _currentDisplay = null;
        }
    }
    
}
