using System;
using System.Collections;
using System.Security.Cryptography;
using EnemyScripts.Zombie;
using UnityEngine;

namespace EnemyScripts
{
    public class ZombieChargeAttackState : BaseState
    {
        [SerializeField] private float attackDuration;
        [SerializeField] private float slowTime;
        [SerializeField] private float stopTime;
        private Vector2 _attackDir;
        private bool _attackFinished;
        private SteeringController _steeringController;
        private Enemy _enemy;
        private Rigidbody2D _playerRb;
        private Rigidbody2D _rb;
        private SpriteRenderer _sr;

        private void Start()
        {
            _steeringController = GetComponentInParent<SteeringController>();
            _enemy = GetComponentInParent<Enemy>();
            _playerRb = _enemy.target.gameObject.GetComponent<Rigidbody2D>();
            _rb = GetComponentInParent<Rigidbody2D>();
            _sr = GetComponentInParent<SpriteRenderer>();
        }

        public override Type Tick()
        {
            if (!_enemy.target)
                return typeof(ZombieIdleState);
            if (_attackFinished)
            {
                _steeringController.frameCount = 0;
                return typeof(ZombieChaseState);
            }
            return null;
        }

        protected override void SwitchState(BaseState state)
        {
            if (state != this) return;
            base.SwitchState(state);
            _attackFinished = false;
            _attackDir = Vector2.zero;
            StartCoroutine(SlowDown());
        }

        private IEnumerator SlowDown()
        {
            float currentSlowTime = slowTime;
            float previousSpeed = _steeringController.speed;
            _sr.color = new Color(1, .75f, .75f);
            while (currentSlowTime > 0)
            {
                currentSlowTime -= Time.fixedDeltaTime;
                _steeringController.speed = _steeringController.speed > 0 ? _steeringController.speed -  2 * previousSpeed * Time.fixedDeltaTime : 0;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
                //if (currentSlowTime < slowTime / 2 && _enemy.target.gameObject.CompareTag("Player") && _attackDir == Vector2.zero)
            }
            CalculateTarget();
            StartCoroutine(Attack());
        }
        
        private void CalculateTarget()
        {
            //predicted position of target after slow down phase
            Vector2 predictPos = _playerRb.velocity * slowTime / 2 + (Vector2)_enemy.target.position;
            //predicted position of target after running to player is complete
            predictPos += _playerRb.velocity * (Vector2.Distance(predictPos, transform.position) / speed);
            _attackDir = predictPos - (Vector2)transform.position;
            _attackDir = _attackDir.normalized;
            Debug.DrawRay(transform.position, _attackDir * 5f, Color.yellow, 1f,false);
        }

        private IEnumerator Attack()
        {
            for (float t = 0; t < attackDuration; t += Time.fixedDeltaTime)
            {
                yield return new WaitForSeconds(Time.fixedDeltaTime);
                _rb.velocity = _attackDir.normalized * (speed * Mathf.Sin((Mathf.PI * t) / attackDuration));
            }
            _sr.color = Color.white;
            yield return new WaitForSeconds(stopTime);
            _attackFinished = true;
        }
        
    }
}