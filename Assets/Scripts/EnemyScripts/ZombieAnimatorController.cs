using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAnimatorController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Animator _anim;
    private string currentState;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        SetAnimation();
    }

    private void SetAnimation()
    {
        var velNorm = _rb.velocity.normalized;
        if (Vector2.Angle(velNorm, Vector2.up) <= 45) {
           PlayAnimationState("ZombieRunUp");
        }
        else if (Vector2.Angle(velNorm, Vector2.right) <= 45) {
            PlayAnimationState("ZombieWalkRight");
        }
        else if (Vector2.Angle(velNorm, Vector2.down) <= 45) {
            PlayAnimationState("ZombieRunDown");
        }
        else if (Vector2.Angle(velNorm, Vector2.left) <= 45) {
            PlayAnimationState("ZombieWalkLeft");
        }
        else {
            PlayAnimationState("ZombieDefault");
        }
    }

    private void PlayAnimationState(string state)
    {
        if (state == currentState) return;
        _anim.Play(state);
        currentState = state;
    }
}
