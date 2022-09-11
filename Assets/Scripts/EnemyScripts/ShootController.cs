using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EnemyScripts
{
    public class ShootController: MonoBehaviour, IFire
    {
        private float _currCooldown;
        private int _currShots;
        private ShootBehaviour _currBehaviour;
        public Transform shootPoint;
        [SerializeField] private ShootBehaviourWeight[] shootBehaviourWeights;
        [SerializeField] private bool shootOnStart;
        private Enemy _enemy;
        
        [System.Serializable]
        private struct ShootBehaviourWeight
        {
            public float weight;
            public ShootBehaviour behaviour;
            public int minShots;
            public int maxShots;
        }

        private bool _canShoot;

        public bool CanShoot
        {
            get => _canShoot;
            set
            {
                //reset shoot cooldown when able to shoot
                if (value && !_canShoot)
                    _currCooldown = 0;
                _canShoot = value;
            }
        }

        private void Awake()
        {
            CanShoot = shootOnStart;
            _enemy = GetComponentInParent<Enemy>();
            PickBehaviour();
        }

        private void FixedUpdate()
        {
            if (!_canShoot || ! _enemy.target) return;
            if (_currCooldown >= _currBehaviour.coolDown)
               Fire();
            else {
                _currCooldown += Time.fixedDeltaTime;
            }
        }
        
        public void Fire()
        {
            _currCooldown = 0;
            _currBehaviour.Shoot(shootPoint, _enemy);
            _currShots--;
            if (_currShots == 0)
                PickBehaviour();
            
        }

        private void PickBehaviour()
        {
            var rand = Random.Range(0f, 1f);
            foreach (var shoot in shootBehaviourWeights)
            {
                if (shoot.weight >= rand)
                {
                    _currBehaviour = shoot.behaviour;
                    _currShots = Random.Range(shoot.minShots, shoot.maxShots + 1);
                    return;
                }
                rand -= shoot.weight;
            }
        }
    }
}