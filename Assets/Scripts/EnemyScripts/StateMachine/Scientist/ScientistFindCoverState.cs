using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EnemyScripts.Scientist
{
    public class ScientistFindCoverState : DistanceBasedState
    {
        [SerializeField] private HumanAnimator moveAnimator;
        [SerializeField] private BaseState inCoverState;
        [SerializeField] private Strafe strafe;
        [SerializeField] private float directionChangeCooldown;
        private bool _inCooldown;

        public override Type Tick()
        {
            if (InCover())
                return inCoverState.GetType();
            moveAnimator.UpdateAnimationAiming();
            return base.Tick();
        }

        public bool InCover()
        {
            var targetPosition = enemy.target.position;
            var centerDir = transform.position - targetPosition;
            var perp = Vector3.Cross(Vector3.forward, centerDir).normalized * .5f;
            var rightDir = transform.position - (targetPosition + perp);
            var leftDir = transform.position - (targetPosition - perp);
            
            var centerHit = Physics2D.Raycast(targetPosition, centerDir,
            Vector2.Distance(targetPosition, (centerDir)) + 1f,
            Physics2D.GetLayerCollisionMask(LayerMask.NameToLayer("Bullet"))).transform == enemy.transform;
            var leftHit = Physics2D.Raycast(targetPosition, leftDir,
            Vector2.Distance(targetPosition, (leftDir)) + 1f,
            Physics2D.GetLayerCollisionMask(LayerMask.NameToLayer("Bullet"))).transform == enemy.transform;
            var rightHit = Physics2D.Raycast(targetPosition, rightDir,
            Vector2.Distance(targetPosition, (rightDir)) + 1f,
            Physics2D.GetLayerCollisionMask(LayerMask.NameToLayer("Bullet"))).transform == enemy.transform;

            return !centerHit && !leftHit && !rightHit;
        }
        
        protected override void SwitchState(BaseState state)
        {
            var rand = Random.Range(0, 2);
            strafe.direction = (int)Mathf.Pow(-1, rand);
            base.SwitchState(state);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_inCooldown)
            {
                strafe.direction *= -1;
                StartCoroutine(WaitCooldown());
            }
        }
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (!_inCooldown)
            {
                strafe.direction *= -1;
                StartCoroutine(WaitCooldown());
            }
        }

        private IEnumerator WaitCooldown()
        {
            _inCooldown = true;
            yield return new WaitForSeconds(directionChangeCooldown);
            _inCooldown = false;
        }
    }
}