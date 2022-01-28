using System;
using System.Collections;
using UnityEngine;

namespace EnemyScripts.Zombie
{
    public class ZombieShootTargetState : BaseState
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Transform shootPoint;
        [SerializeField] private SpriteRenderer sr;
        [SerializeField] private BarfZombieAnimatorController animController;
        [SerializeField] private float attackCoolDown = 3f;
        private const string BarfHoriz = "BarfHoriz";
        private const string BarfUp = "BarfUp";
        private const string BarfDown = "BarfDown";
        [SerializeField] private BaseState afterAttackState;
        private Type _nextState;
        private string _currAttackAnimationName;
        private Enemy _enemy;
        private static readonly int AttackOver = Animator.StringToHash("AttackOver");
        private static readonly int FacingDown = Animator.StringToHash("FacingDown");
        private static readonly int FacingUp = Animator.StringToHash("FacingUp");
        private static readonly int FacingSideways = Animator.StringToHash("FacingSideways");

        protected override void Awake()
        {
            base.Awake();
            _enemy = transform.parent.GetComponent<Enemy>();

        }

        public override Type Tick()
        {
            return _nextState;
        }

        protected override void SwitchState(BaseState state)
        {
            base.SwitchState(state);
            if (state.GetType() == GetType())
            {
                _nextState = null;
                animController.animate = false;
                Vector2 dir =  _enemy.target.transform.position - shootPoint.position;
                if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                {
                    _currAttackAnimationName = BarfHoriz;
                    sr.flipX = dir.x < 0;
                }
                else if (dir.y > 0)
                    _currAttackAnimationName = BarfUp;
                else
                    _currAttackAnimationName = BarfDown;
                animator.Play(_currAttackAnimationName);
                animator.SetBool(FacingDown, false);
                animator.SetBool(FacingUp, false);
                animator.SetBool(FacingSideways, false);
                StartCoroutine(WaitForAttackEnd());
            }
            else
                animController.animate = true;
        }

        private IEnumerator WaitForAttackEnd()
        {
            yield return new WaitUntil(() =>
                !animator.GetCurrentAnimatorStateInfo(0).IsName(_currAttackAnimationName));
            yield return new WaitForSeconds(attackCoolDown);
            animator.SetTrigger(AttackOver);
            _nextState = afterAttackState.GetType();

        }
    }
}