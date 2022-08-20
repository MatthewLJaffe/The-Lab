using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace EnemyScripts.Agent
{
    public class RollState : BaseState
    {
        [SerializeField] private BaseState[] overridableStates;
        [SerializeField] private float rollCooldown;
        [SerializeField] private float rollCheckDelay;
        [SerializeField] private float rollChance;
        [SerializeField] private float rollDuration;
        [SerializeField] private float maxRollSpeed;
        [SerializeField] private AnimationCurve rollCurve;
        [SerializeField] private Rigidbody2D enemyRb;
        [SerializeField] private Vector2[] rollDirs;
        [SerializeField] private UnityEvent rollBegin;
        [SerializeField] private UnityEvent rollEnd;
        [SerializeField] private Animator anim;
        private bool _inCooldown;
        private bool _inDelay;
        private bool _rolling;
        private Vector2 _startVelocity;
        private BaseState _prevState;
        
        public override Type Tick()
        {
            if (_rolling)
                return null;
            return _prevState.GetType();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_inDelay || _inCooldown || _rolling || other.gameObject.layer != LayerMask.NameToLayer("Bullet"))
                return;
            StartCoroutine(Delay());
            if (Random.Range(0f, 1f) > rollChance) return;
            foreach (var state in overridableStates) {
                state.OverrideState = this;
            }
        }

        protected override void SwitchState(BaseState state)
        {
            if (state == this)
                InitiateRoll();
            else
            {
                _prevState = state;
                base.SwitchState(state);
            }
        }

        private void InitiateRoll()
        {
            rollBegin.Invoke();
            _steeringController.enabled = false;
            _startVelocity = enemyRb.velocity;
            foreach (var state in overridableStates) {
                state.OverrideState = null;
            }
            var maxDot = -99f;
            var bestDir = rollDirs[0];
            foreach (var dir in rollDirs)
            {
                var dot = Vector2.Dot(dir, _startVelocity);
                if (dot > maxDot) {
                    maxDot = dot;
                    bestDir = dir;
                }
            }
            StartCoroutine(Roll(bestDir));
        }

        private IEnumerator Roll(Vector2 dir)
        {
            _rolling = true;
            transform.parent.gameObject.layer = LayerMask.NameToLayer("Invincible");
            for (var t = 0f; t < rollDuration; t += Time.fixedDeltaTime) {
                enemyRb.velocity = dir * (maxRollSpeed * rollCurve.Evaluate(t / rollDuration));
                yield return new WaitForFixedUpdate();
            }
            transform.parent.gameObject.layer = LayerMask.NameToLayer("Enemy");
            _rolling = false;
            rollEnd.Invoke();
            _steeringController.enabled = true;
            StartCoroutine(Cooldown());
        }

        private IEnumerator Delay()
        {
            _inDelay = true;
            yield return new WaitForSeconds(rollCheckDelay);
            _inDelay = false;
        }

        private IEnumerator Cooldown()
        {
            _inCooldown = true;
            yield return new WaitForSeconds(rollCooldown);
            _inCooldown = false;
        }
    }
}