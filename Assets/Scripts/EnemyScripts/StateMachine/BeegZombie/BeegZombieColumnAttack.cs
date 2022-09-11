using System;
using UnityEngine;
using UnityEngine.Events;

namespace EnemyScripts.BeegZombie
{
    public class BeegZombieColumnAttack : BeegZombieAttackState
    {
        public UnityEvent onColumSpawn;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private BeegZombieAnimator animator;
        [SerializeField] private SteeringController steeringController;
        [SerializeField] private BaseState trackTargetState;
        [SerializeField] private float attackDuration;
        [SerializeField] private GameObject column;
        [SerializeField] private Transform footPos;
        [SerializeField] private float maxColumnDistance;
        [SerializeField] private float columnSpeed;
        [SerializeField] private float columnDamage;
        private Enemy _enemy;
        private float _currTime;

        protected override void Awake()
        {
            _enemy = GetComponentInParent<Enemy>();
            base.Awake();
        }

        public override Type Tick()
        {
            if (_currTime > attackDuration)
            {
                steeringController.enabled = true;
                return trackTargetState.GetType();
            }
            _currTime += Time.deltaTime;
            return null;
        }

        protected override void SwitchState(BaseState state)
        {
            base.SwitchState(state);
            if (state == this)
            {
                _currTime = 0;
                steeringController.enabled = false;
                rb.velocity = Vector2.zero;
                animator.currAttack = this;
                animator.PlayStomp();
            }
        }

        public void SpawnColumn()
        {
            onColumSpawn.Invoke();
            var startPos = footPos.position;
            var columnDir = (_enemy.target.position - startPos).normalized;
            var endPoint = startPos + columnDir * maxColumnDistance;
            var hit = Physics2D.Raycast(startPos, columnDir, maxColumnDistance,
                LayerMask.GetMask( "Default"));
            if (hit)
                endPoint = hit.point;
            var columnAttack = Instantiate(column, startPos, Quaternion.identity).GetComponent<ColumnAttack>();
            columnAttack.destination = endPoint;
            columnAttack.speed = columnSpeed;
            columnAttack.damage = columnDamage;
        }
    }
}