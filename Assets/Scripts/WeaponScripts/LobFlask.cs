using EnemyScripts;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace WeaponScripts
{
    public class LobFlask : MonoBehaviour, IFire
    {
        [SerializeField] private GameObject flask;
        [SerializeField] private Enemy enemy;
        [SerializeField] private Transform shootPoint;
        [SerializeField] private float predictFactor;
        private Rigidbody2D _targetRb;
        [SerializeField] private float flaskSpeed;
        [SerializeField] private float projectileSpeed;
        [SerializeField] private Vector2 cooldownRange;
        private float _currCooldown;
        public UnityEvent onLob;
        public float fireProb = 1;
        public float FireProb { get; set; }
        public bool CanShoot { get; set; }
        
        private void Start()
        {
            CanShoot = true;
            _currCooldown = Random.Range(cooldownRange.x, cooldownRange.y);
        }

        private void FixedUpdate()
        {
            if(!CanShoot) return;
            if (0 >= _currCooldown) {
                if (Random.Range(0f, 1f) < fireProb)
                    Fire();
                _currCooldown = Random.Range(cooldownRange.x, cooldownRange.y);
            }
            else
                _currCooldown -= Time.fixedDeltaTime;
        }

        public void Fire()
        {
            onLob.Invoke();
            var flaskComponent = Instantiate(flask, shootPoint.position, Quaternion.identity)
                .GetComponentInChildren<ScientistFlask>();
            flaskComponent.projectileSpeed = projectileSpeed;
            flaskComponent.flying = true;
            flaskComponent.speed = flaskSpeed;
            flaskComponent.enemy = enemy;
            Vector2 dir = transform.position - enemy.target.position;
            if (_targetRb == null)
                _targetRb = enemy.target.GetComponent<Rigidbody2D>();
            flaskComponent.destination = 
                _targetRb.velocity * (dir.magnitude * predictFactor / flaskSpeed) + (Vector2)enemy.target.position;
        }

    }
}