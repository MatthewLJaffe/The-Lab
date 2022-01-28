using System;
using UnityEngine;

namespace EnemyScripts
{
    public class BarfZombieAnimatorController : MonoBehaviour
    {
        private SpriteRenderer _sr;
        private Animator _animator;
        [SerializeField] private Rigidbody2D _rb;
        public bool animate = true;
        private static readonly int FacingSideways = Animator.StringToHash("FacingSideways");
        private static readonly int FacingUp = Animator.StringToHash("FacingUp");
        private static readonly int FacingDown = Animator.StringToHash("FacingDown");
        [SerializeField] private int projectileSortingOrder = 3;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody2D>();
            _sr = GetComponent<SpriteRenderer>();
        }

        private void FixedUpdate()
        {
            if (animate)
                SetAnimationState();
        }

        private void SetAnimationState()
        {
            if (_rb.velocity.magnitude < .1f)
            {
                _animator.SetBool(FacingSideways, false);
                _animator.SetBool(FacingUp, false);
                _animator.SetBool(FacingDown, false);
                return;
            }
            
            if (Mathf.Abs(_rb.velocity.x) > Mathf.Abs(_rb.velocity.y))
            {
                _sr.sortingOrder = projectileSortingOrder - 1;
                _sr.flipX = _rb.velocity.x < 0;
                _animator.SetBool(FacingSideways, true);
                _animator.SetBool(FacingUp, false);
                _animator.SetBool(FacingDown, false);
            }
            else if (_rb.velocity.y > 0)
            {
                _sr.sortingOrder = projectileSortingOrder + 1;
                _animator.SetBool(FacingSideways, false);
                _animator.SetBool(FacingUp, true);
                _animator.SetBool(FacingDown, false);
            }
            else
            {
                _sr.sortingOrder = projectileSortingOrder - 1;
                _animator.SetBool(FacingSideways, false);
                _animator.SetBool(FacingUp, false);
                _animator.SetBool(FacingDown, true);
            }
        }
    }
}