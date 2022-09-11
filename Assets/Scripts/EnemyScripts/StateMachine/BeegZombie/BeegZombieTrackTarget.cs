using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EnemyScripts.BeegZombie
{
    public class BeegZombieTrackTarget : BaseState
    {
        
        [SerializeField] private float closeDistance;
        [SerializeField] private AttackWeight[] attackWeights;
        [SerializeField] private BeegZombieAnimator beegAnimator;
        private BeegZombieAttackState _lastAttack;
        private Enemy _enemy;
        private float _attackCooldown;

        protected override void Awake()
        {
            _enemy = GetComponentInParent<Enemy>();
            base.Awake();
            _attackCooldown = 4f;
        }

        [Serializable] 
        struct AttackWeight
        {
            public float weight;
            public BeegZombieAttackState attack;
        }

        public override Type Tick()
        {
            if (_attackCooldown > 0 || 
                Vector2.Distance(_enemy.target.position, transform.position) > closeDistance)
            {
                beegAnimator.UpdateWalkAnimation();
                _attackCooldown -= Time.deltaTime;
                return null;
            }
            return PickAttackState().GetType();
            
        }

        private BaseState PickAttackState()
        {
            var rand = Random.Range(0f, 1f);
            foreach (var aw in attackWeights)
            {
                if (aw.weight > rand)
                {
                    _lastAttack = aw.attack;
                    return aw.attack;
                }
                rand -= aw.weight;
            }
            return attackWeights[attackWeights.Length - 1].attack;
        }

        protected override void SwitchState(BaseState state)
        {
            if (state == this && _lastAttack)
                _attackCooldown = _lastAttack.attackCooldown;
            base.SwitchState(state);
        }
    }
}