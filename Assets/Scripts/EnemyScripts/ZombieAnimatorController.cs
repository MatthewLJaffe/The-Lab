﻿using UnityEngine;

namespace EnemyScripts
{
    public class ZombieAnimatorController : MonoBehaviour
    {
        private Rigidbody2D _rb;
        private Animator _anim;
        private SpriteRenderer _sr;
        private string _currentState;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();
            _sr = GetComponent<SpriteRenderer>();
        }

        private void FixedUpdate()
        {
            SetAnimation();
        }

        private void SetAnimation()
        {
            var velNorm = _rb.velocity.normalized;
            if (velNorm == Vector2.zero) return;
            if (Vector2.Angle(velNorm, Vector2.up) <= 45) {
                PlayAnimationState("ZombieRunUp");
            }
            else if (Vector2.Angle(velNorm, Vector2.right) <= 45 || Vector2.Angle(velNorm, Vector2.left) <= 45) {
                PlayAnimationState("ZombieWalkRight");
            }
            else if (Vector2.Angle(velNorm, Vector2.down) <= 45) {
                PlayAnimationState("ZombieRunDown");
            }
            _anim.SetBool("Forward", velNorm.y < 0);
            _sr.flipX = velNorm.x < 0;
        }

        private void PlayAnimationState(string state)
        {
            if (state == _currentState) return;
            _anim.Play(state);
            _currentState = state;
        }
    }
}
