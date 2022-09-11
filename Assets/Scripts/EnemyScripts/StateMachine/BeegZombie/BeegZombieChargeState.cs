using System;
using CameraScripts;
using General;
using UnityEngine;

namespace EnemyScripts.BeegZombie
{
    public class BeegZombieChargeState : BeegZombieAttackState
    {
        [SerializeField] private SoundEffect screenShakeSfx;
        [SerializeField] private ShakeScreen shakeScreen;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private BaseState trackTargetState;
        [SerializeField] private BaseState stunnedState;
        [SerializeField] private float predictFactor;
        [SerializeField] private AnimationCurve chargeCurve;
        [SerializeField] private BeegZombieAnimator beegAnimator;
        [SerializeField] private float maxChargeSpeed;
        [SerializeField] private SteeringController steeringController;
        public bool charging;
        private float _chargeTime;
        private float _currTime;
        private Vector2 _chargeDir;
        private Enemy _enemy;

        protected override void Awake()
        {
            _enemy = GetComponentInParent<Enemy>();
            base.Awake();
        }
        
        public override Type Tick()
        {
            if (_currTime > _chargeTime)
            {
                charging = false;
                steeringController.enabled = true;
                return trackTargetState.GetType();
            }
            if (!charging) return null;
            
            rb.velocity = _chargeDir * (maxChargeSpeed * chargeCurve.Evaluate(_currTime / _chargeTime));
            _currTime += Time.deltaTime;
            return null;
        }

        protected override void SwitchState(BaseState state)
        {
            if (state == this)
                StartCharge();
            base.SwitchState(state);
        }

        private void StartCharge()
        {
            steeringController.enabled = false;
            rb.velocity = Vector2.zero;
            _currTime = 0;
            ComputeChargeDir();
            beegAnimator.PlayChargeAnimation(_chargeDir);
        }

        public void ComputeChargeDir()
        {
            _chargeDir = _enemy.target.position - transform.position;
            var distance = _chargeDir.magnitude;
            var enemyRb = _enemy.target.GetComponent<Rigidbody2D>();
            _chargeDir += enemyRb.velocity * (predictFactor * distance) / maxChargeSpeed;
            _chargeTime = 2 * _chargeDir.magnitude / maxChargeSpeed;
            _chargeDir.Normalize();
        }

        public void DetectChargeCollision()
        {
            if (!charging) return;
            charging = false;
            steeringController.enabled = true;
            rb.velocity = Vector2.zero;
            shakeScreen.Shake();
            screenShakeSfx.Play();
            OverrideState = stunnedState;
        }
    }
}