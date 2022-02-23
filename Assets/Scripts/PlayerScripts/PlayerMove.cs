using System;
using EntityStatsScripts;
using UnityEngine;

namespace PlayerScripts
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 4.5f;
        private Rigidbody2D _rb;
        private Vector2 _dir;

        private void Awake() {
            _rb = GetComponent<Rigidbody2D>();
            PlayerStats.OnStatChange += ChangeSpeed;
        }
        
        private void OnDestroy()
        {
            PlayerStats.OnStatChange -= ChangeSpeed;
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
            _dir.Normalize();
        }

        private void OnDisable()
        {
            _rb.velocity = Vector2.zero;
        }

        private void FixedUpdate()
        {
            _rb.velocity = new Vector2(_dir.x, _dir.y) * moveSpeed;
        }
    }
}
