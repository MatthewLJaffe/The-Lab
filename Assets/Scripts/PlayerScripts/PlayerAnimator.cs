using System;
using UnityEngine;

namespace PlayerScripts
{
    public class PlayerAnimator : MonoBehaviour
    {
        private Animator _anim;
        private SpriteRenderer _sr;
        private Rigidbody2D _rb;
        private Camera _mainCamera;
        private bool _animate = true;
        [SerializeField] private DirectionState[] directionStates;
        private DirectionState _currState;
        [System.Serializable]
        private struct DirectionState
        {
            public Vector2 dir;
            public string stateName;
        }
        
        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody2D>();
            _sr = GetComponent<SpriteRenderer>();
            _mainCamera = Camera.main;
            PlayerRoll.onRoll += RollHandler;
        }

        private void OnDestroy()
        {
            PlayerRoll.onRoll -= RollHandler;
        }

        private void Update()
        {
            if (_animate)
                SetAnimation();
        }

        private void RollHandler(bool rolling, Vector2 dir)
        {
            _animate = !rolling;
            if (rolling)
            {
                if (dir == Vector2.up)
                {
                    if ((_mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position).x < 0)
                        _anim.Play("RollUpLeft");
                    else
                        _anim.Play("RollUpRight");
                }
                else if (dir == Vector2.down)
                {
                    if ((_mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position).x < 0)
                        _anim.Play("RollDownLeft");
                    else
                        _anim.Play("RollDownRight");
                }
                else
                {
                    var theta = Vector2.SignedAngle(dir, Vector2.up);
                    var rollState = "Roll" + GetDirectionState(theta).stateName;
                    _anim.Play(rollState);
                }

            }
        }
        
        private void SetAnimation()
        {
            _currState = 
                GetDirectionState(Vector2.SignedAngle(_mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position, Vector2.up));
            var currAnim = "Idle" + _currState.stateName;
            if (_rb.velocity.magnitude > .1f)
            {
                currAnim = currAnim.Replace("Idle", "Run");
                if (Vector2.Dot(_currState.dir, _rb.velocity) < 0)
                    currAnim = currAnim.Replace("Run", "Reverse");
            }
            _anim.Play(currAnim);
        }

        private DirectionState GetDirectionState (float theta)
        {
            if (theta > 0 && theta <= 90)
                return directionStates[0];
            if (theta > 90 && theta <= 180)
                return directionStates[1];
            if (theta > -180 && theta <= -90)
                return directionStates[2];
            if (theta > -90 && theta <= 0)
                return directionStates[3];
            return directionStates[0];
        }
        
    }
}
