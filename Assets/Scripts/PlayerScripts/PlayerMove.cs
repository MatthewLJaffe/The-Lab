using System;
using EntityStatsScripts;
using UnityEngine;

namespace PlayerScripts
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 4.5f;
        public static Action moveTick = delegate {  };
        private Rigidbody2D _rb;
        private Vector2 _dir;
        private bool _walking = true;
        private float _moveTick;

        private void Awake() {
            _rb = GetComponent<Rigidbody2D>();
            PlayerStats.onStatChange += ChangeSpeed;
            PlayerRoll.onRoll += (roll, d) => _walking = !roll;
        }
        
        private void OnDestroy()
        {
            PlayerStats.onStatChange -= ChangeSpeed;
        }

        private void ChangeSpeed(PlayerStats.StatType type, float newSpeed)
        {
            if (type == PlayerStats.StatType.Speed)
                moveSpeed = newSpeed;
        }
        
        private void Update()
        {
            _dir.x = Input.GetAxis("Horizontal");
            _dir.y = Input.GetAxis("Vertical");
            _dir = _dir.normalized;
        }

        private void OnDisable()
        {
            _rb.velocity = Vector2.zero;
        }
        private void FixedUpdate()
        {
            if (_walking)
                _rb.velocity = new Vector2(_dir.x, _dir.y) * moveSpeed;
            
            if (_rb.velocity.magnitude <= .1f) return;
            _moveTick += Time.fixedDeltaTime;
            if (_moveTick > 1f)
            {
                _moveTick = 0f;
                moveTick.Invoke();
            }
        }
    }
}
