using System;
using System.Collections;
using EntityStatsScripts.Effects;
using General;
using PlayerScripts;
using TMPro;
using UnityEngine;
using static Unity.Mathematics.Random;
using Random = UnityEngine.Random;

namespace EntityStatsScripts
{
    public class PlayerHealthBar : PlayerBar, IDamageable
    {
        public float defense;
        public float dodgeChance;
        [SerializeField] private float damageCooldown;
        [SerializeField] private GameObject numberPrefab;
        [SerializeField] private Transform displayPoint;
        [SerializeField] private float timeUntilAdrenaline = 100;
        [SerializeField] private Effect adrenaline;
        [SerializeField] private Animator animator;
        private float _currAddyCount;
        private bool _canBeDamaged = true;
        private Coroutine _currentDisplay;
        private GameObjectPool _damageNumberPool;
        private SpriteRenderer _sr;
        private Coroutine _adrenalineRoutine;

        public override float BarValue
        {
            get => barValue;
            set
            {
                base.BarValue = value;
                var precentage = barValue / maxValue;
                if (_adrenalineRoutine != null)
                    StopCoroutine(_adrenalineRoutine);
                if (precentage <= .25f)
                    _adrenalineRoutine = StartCoroutine(CountToAdrenaline(2));
                else if (precentage < .5f)
                    _adrenalineRoutine = StartCoroutine(CountToAdrenaline(1));
            }
        }

        private void Awake()
        {
            DamagePlayer.applyPlayerDamage += TakeDamage;
            _sr = GetComponentInParent<SpriteRenderer>();
            _damageNumberPool = new GameObjectPool(numberPrefab, displayPoint);
            BarDeplete += KillPlayer;
            PlayerStats.OnStatChange += ChangeStats;
        }

        private void OnDestroy()
        {
            DamagePlayer.applyPlayerDamage -= TakeDamage;
            PlayerStats.OnStatChange -= ChangeStats;
            BarDeplete -= KillPlayer;
        }
        
        public void TakeDamage(float amount, Vector2 dir)
        {
            if (!_canBeDamaged) return;
            if (Random.Range(0, 100) < dodgeChance)
            {
                StartCoroutine(WaitDamageCooldown());
                StartCoroutine(TakeDamageEffect(_damageNumberPool.GetFromPool().GetComponent<TextMeshProUGUI>(), "Dodged"));
            }
            else
            {
                var damageAmount = amount / 2f / (defense * .25f + 1) + Mathf.Max(amount / 2 - defense / 2, 0);
                var roundedDamage = Mathf.Round(damageAmount) >= 1 ? Mathf.Round(damageAmount) : 1;
                BarValue -= roundedDamage;
                StartCoroutine(WaitDamageCooldown());
                StartCoroutine(TakeDamageEffect(_damageNumberPool.GetFromPool().GetComponent<TextMeshProUGUI>(), roundedDamage.ToString()));
            }
        }
        
        private IEnumerator WaitDamageCooldown()
        {
            _canBeDamaged = false;
            yield return new WaitForSeconds(damageCooldown);
            _canBeDamaged = true;
        }

        private IEnumerator TakeDamageEffect(TextMeshProUGUI damageText, string amount)
        {
            damageText.text = amount;
            gameObject.transform.root.gameObject.layer = LayerMask.NameToLayer("Invincible");
            while (!_canBeDamaged)
            {
                var color = _sr.color;
                color.a = Math.Abs(color.a - 1) < .01f ? .75f : 1f;
                _sr.color = color;
                yield return new WaitForSeconds(damageCooldown/4);
            }
            gameObject.transform.root.gameObject.layer = LayerMask.NameToLayer("Player");
        }

        private void ChangeStats(PlayerStats.StatType type, float newValue)
        {
            switch (type)
            {
                case PlayerStats.StatType.MaxHealth:
                    MaxValue = newValue;
                    break;
                case PlayerStats.StatType.Defense:
                    defense = newValue;
                    break;
                case PlayerStats.StatType.DodgeChance:
                    dodgeChance = newValue;
                    break;
            }
        }

        private void KillPlayer(PlayerBarType barType)
        {
            animator.SetTrigger("Kill");
        }

        public void DestroyPlayer()
        {
            PlayerFind.instance.DestroyPlayer();
        }

        private IEnumerator CountToAdrenaline(float increment)
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                _currAddyCount += increment;
                if (!(_currAddyCount > timeUntilAdrenaline)) continue;
                adrenaline.Stack ++;
                _currAddyCount = 0;
            }
        }
    }
}
