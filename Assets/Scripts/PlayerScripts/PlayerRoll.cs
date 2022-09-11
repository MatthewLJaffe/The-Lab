using System;
using System.Collections;
using UnityEngine;

namespace PlayerScripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerRoll : MonoBehaviour
    {
        public static Action<bool, Vector2> onRoll = delegate { };
        [SerializeField] private float cooldownTime;
        [SerializeField] private float maxRollSpeed;
        [SerializeField] private AnimationCurve rollCurve;
        [SerializeField] private float rollTime;
        [SerializeField] private float invincibleTime;
        [SerializeField] private Vector2[] rollDirections;
        public bool invulnerable;
        private Rigidbody2D _rb;
        private bool _inCooldown;
        

        private bool _rolling;

        private void Awake()
        {
            PlayerInputManager.onInputDown += Roll;
            _rb = GetComponent<Rigidbody2D>();
        }

        private void OnDestroy()
        {
            PlayerInputManager.onInputDown -= Roll;
        }

        //Called by animation event
        public void StopRoll()
        {
            _rolling = false;
            onRoll.Invoke(false, Vector2.zero);
            StartCoroutine(CooldownRoutine());
        }

        private void Roll(PlayerInputManager.PlayerInputName iName)
        {
            if (iName != PlayerInputManager.PlayerInputName.Roll || _inCooldown || _rolling) return;
            _rolling = true;
            var inputDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
            Vector2 rollDir = GetRollDir(inputDir).normalized;
            onRoll.Invoke(true, rollDir);
            StartCoroutine(ApplyRoll(rollDir));
        }

        private Vector2 GetRollDir(Vector2 inputDir)
        {
            var rollDir = rollDirections[0];
            float minAngle = 361;
            foreach (var d in rollDirections)
            {
                var angle = Vector2.Angle(d, inputDir);
                if ( angle < minAngle)
                {
                    rollDir = d;
                    minAngle = angle;
                }
            }
            return rollDir;
        }

        private IEnumerator CooldownRoutine()
        {
            _inCooldown = true;
            yield return new WaitForSeconds(cooldownTime);
            _inCooldown = false;
        }

        private IEnumerator ApplyRoll(Vector2 dir)
        {
            var peaked = false;
            var fixedUpdate = new WaitForFixedUpdate();
            gameObject.layer = LayerMask.NameToLayer("Invincible");
            for (var t = 0f; t < rollTime; t += Time.fixedDeltaTime)
            {
                if (t > invincibleTime && !invulnerable)
                    gameObject.layer = LayerMask.NameToLayer("Player");
                var rollSpeed = rollCurve.Evaluate(t / rollTime) * maxRollSpeed;
                //if roll speed is quicker use that
                if (rollSpeed > _rb.velocity.magnitude)
                {
                    peaked = true;
                    _rb.velocity = dir * rollSpeed;
                }
                //if run speed is quicker use that only if we are in the startup of the animation
                else
                {
                    if (peaked)
                        _rb.velocity = dir * rollSpeed;
                    else
                        _rb.velocity = dir * _rb.velocity.magnitude;
                }
                yield return fixedUpdate;
            }
        }
        
    }
}
