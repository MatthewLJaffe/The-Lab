using System;
using UnityEngine;

namespace EnemyScripts
{
    /// <summary>
    /// rotates transform to look at target
    /// </summary>
    public class LookAtTarget : MonoBehaviour
    {
        private Enemy _enemy;

        private void Awake()
        {
            _enemy = GetComponentInParent<Enemy>();
            _enemy.enemyKilled += delegate { enabled = false; };
        }

        private void FixedUpdate()
        {
            if (!_enemy.target) return;
            var trans = transform;
            trans.up = trans.position - _enemy.target.position;
        }
    }
}