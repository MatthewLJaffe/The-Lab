using System;
using System.Collections;
using EntityStatsScripts.Effects;
using General;
using PlayerScripts;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using WeaponScripts;
using Random = UnityEngine.Random;

namespace EntityStatsScripts
{
    /// <summary>
    /// controls health bar value and effects that are tied to health bar amount
    /// </summary>
    public class PlayerHealthBar : PlayerBar, IDamageable
    {
        public UnityEvent onDamage;
        public UnityEvent onReverseUno;
        public float defense;
        public float dodgeChance;
        [SerializeField] private PlayerAnimator playerAnimator;
        [SerializeField] private Animator animator;
        [SerializeField] private float damageCooldown;
        [SerializeField] private GameObject numberPrefab;
        [SerializeField] private Transform displayPoint;
        [SerializeField] private float timeUntilAdrenaline = 100;
        [SerializeField] private Effect adrenaline;
        [SerializeField] private ReverseUnoCardEffect reverseUno;
        [SerializeField] private PlayerRoll playerRoll;
        private float _currAddyCount;
        private Coroutine _currentDisplay;
        private GameObjectPool _damageNumberPool;
        private SpriteRenderer _sr;
        private Coroutine _adrenalineRoutine;
        private float _regenPerTick;

        public override float BarValue
        {
            get => barValue;
            set
            {
                base.BarValue = value;
                if (_adrenalineRoutine != null)
                    StopCoroutine(_adrenalineRoutine);
                if (barValue <= barVeryLowValue)
                    _adrenalineRoutine = StartCoroutine(CountToAdrenaline(2));
                else if (barValue < barLowValue)
                    _adrenalineRoutine = StartCoroutine(CountToAdrenaline(1));
            }
        }

        private void Awake()
        {
            DamagePlayer.applyPlayerDamage += TakeDamage;
            _sr = GetComponentInParent<SpriteRenderer>();
            _damageNumberPool = new GameObjectPool(numberPrefab, displayPoint);
            BarDeplete += KillPlayer;
            PlayerStats.onStatChange += ChangeStats;
            PlayerMove.moveTick += TickRegen;
        }

        private void OnDestroy()
        {
            DamagePlayer.applyPlayerDamage -= TakeDamage;
            PlayerStats.onStatChange -= ChangeStats;
            BarDeplete -= KillPlayer;
            PlayerMove.moveTick -= TickRegen;
        }
        
        public void TakeDamage(float amount, Vector2 dir, DamageSource source,  bool crit = false)
        {
            if (reverseUno.RollReverse())
            {
                if (source)
                {
                    source.GetComponentInChildren<IDamageable>()?.TakeDamage(amount, dir);
                    var bullet = source.gameObject.GetComponent<Bullet>();
                    if (bullet)
                        bullet.firedBy.GetComponentInChildren<IDamageable>()?.TakeDamage(amount, dir);
                }
                onReverseUno.Invoke();
                StartCoroutine(WaitDamageCooldown());
                StartCoroutine(TakeDamageEffect(_damageNumberPool.GetFromPool().GetComponent<TextMeshProUGUI>(), ""));
            }
            else if (Random.Range(0, 100) < dodgeChance)
            {
                StartCoroutine(WaitDamageCooldown());
                StartCoroutine(TakeDamageEffect(_damageNumberPool.GetFromPool().GetComponent<TextMeshProUGUI>(), "Dodged"));
            }
            else
            {
                onDamage.Invoke();
                var damageAmount = amount / 2f / (defense * .25f + 1) + Mathf.Max(amount / 2 - defense / 2, 0);
                var roundedDamage = Mathf.Round(damageAmount) >= 1 ? Mathf.Round(damageAmount) : 1;
                BarValue -= roundedDamage;
                StartCoroutine(WaitDamageCooldown());
                StartCoroutine(TakeDamageEffect(_damageNumberPool.GetFromPool().GetComponent<TextMeshProUGUI>(), roundedDamage.ToString()));
            }
        }
        
        private IEnumerator WaitDamageCooldown()
        {
            gameObject.transform.root.gameObject.layer = LayerMask.NameToLayer("Invincible");
            playerRoll.invulnerable = true;
            yield return new WaitForSeconds(damageCooldown);
            gameObject.transform.root.gameObject.layer = LayerMask.NameToLayer("Player");
            playerRoll.invulnerable = false;
        }

        private IEnumerator TakeDamageEffect(TextMeshProUGUI damageText, string amount)
        {
            damageText.text = amount;
            for (var t = 0f; t < damageCooldown; t += damageCooldown/4)
            {
                var color = _sr.color;
                color.a = Math.Abs(color.a - 1) < .01f ? .75f : 1f;
                _sr.color = color;
                yield return new WaitForSeconds(damageCooldown/4);
            }
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
                case PlayerStats.StatType.RegenPerTick:
                    _regenPerTick = newValue;
                    break;
            }
        }
        
        private void TickRegen()
        {
            BarValue += _regenPerTick;
        }


        private void KillPlayer(PlayerBarType barType)
        {
            playerAnimator.enabled = false;
            animator.Play("Die");
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
