using System;
using System.Collections;
using EntityStatsScripts;
using General;
using PlayerScripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WeaponScripts
{
    public class Gun : MonoBehaviour
    {
        public static Action<int, int> BroadcastShot = delegate { };

        [SerializeField] protected PlayerStats playerStats;
        [SerializeField] protected GameObject bullet;
        [SerializeField] protected GunStats stats;
        [SerializeField] protected Transform shootPoint;
        private bool firstEquip = true;
        private Transform _playerTrans;
        protected int currentMagSize;
        protected bool reloading;
        protected bool firing;
        protected static float maxReloadTime = 3;
        protected static float minReloadTime = .1f;
        protected GameObjectPool _bulletPool;
        protected float additionalDamage;
        protected float playerCritChance;
        protected float additionalAccuracy;
        protected float additionalFireRate;

        protected void Start() 
        {
            currentMagSize = stats.magSize;
            BroadcastShot(currentMagSize, stats.magSize);
            firstEquip = false;
            _bulletPool = new GameObjectPool(bullet);
            _playerTrans = PlayerFind.Instance.playerInstance.transform;
            additionalDamage = playerStats.PlayerStatsDict[PlayerStats.StatType.Attack].CurrentValue;
            playerCritChance = playerStats.PlayerStatsDict[PlayerStats.StatType.CritChance].CurrentValue;
            additionalAccuracy = playerStats.PlayerStatsDict[PlayerStats.StatType.Accuracy].CurrentValue;
            additionalFireRate = playerStats.PlayerStatsDict[PlayerStats.StatType.FireRate].CurrentValue;
            PlayerStats.OnStatChange += delegate(PlayerStats.StatType type, float newValue) 
            {
                switch (type)
                {
                    case PlayerStats.StatType.Attack:
                        additionalDamage = newValue;
                        break;
                    case PlayerStats.StatType.CritChance:
                        playerCritChance = newValue;
                        break;
                    case  PlayerStats.StatType.Accuracy:
                        additionalAccuracy = newValue;
                        Debug.Log("ADDITIONAL ACCURACY UPDATED " + additionalAccuracy);
                        break;
                    case PlayerStats.StatType.FireRate:
                        additionalFireRate = newValue;
                        break;
                }
            };
        }

        protected void OnEnable() {
            if (!firstEquip)
                BroadcastShot(currentMagSize, stats.magSize);
            PlayerInputManager.OnInputDown += StartReload;
        }
        
        private void OnDisable()
        {
            reloading = false;
            firing = false;
            PlayerInputManager.OnInputDown -= StartReload;
        }

        protected void StartReload(PlayerInputManager.PlayerInputName iName)
        {
            if (iName == PlayerInputManager.PlayerInputName.Reload)
                StartCoroutine(Reload());
        }
        
        
        protected void Update() {
            if (PlayerInputManager.instance.GetInput(PlayerInputManager.PlayerInputName.Fire1))
                StartCoroutine(Fire());
        }

        protected IEnumerator Fire()
        {
            if (reloading || firing)
                yield break;
            for (int i = 0; i < stats.projectiles; i++)
            {
                GameObject bulletInstance = _bulletPool.GetFromPool();
                bulletInstance.transform.position = shootPoint.position;
                var bulletComponent = bulletInstance.GetComponent<Bullet>();
                var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                bulletComponent.damage = stats.damage + stats.damage * (additionalDamage / 10);
                if (stats.critChance + playerCritChance > Random.Range(0f, 100f))
                    bulletComponent.damage *= 2;
                bulletComponent.accuracy = Mathf.Clamp(stats.accuracy + additionalAccuracy, 0f, 100f);
                bulletComponent.speed = bulletComponent.accuracy / 8f + 4;
                if (Vector2.Distance(mousePos, _playerTrans.position) > 1)
                    bulletComponent.direction = mousePos - shootPoint.position;
                else
                    bulletComponent.direction = mousePos - _playerTrans.position;
                
            }
            currentMagSize--;
            if (currentMagSize == 0)
                StartCoroutine(Reload());
            BroadcastShot(currentMagSize, stats.magSize);
            firing = true;
            yield return new WaitForSeconds(60 / (stats.fireRate + stats.fireRate * additionalFireRate / 10));
            firing = false;
        }
        
        protected IEnumerator Reload()
        {
            if (reloading || firing)
                yield break;
            reloading = true;
            yield return new WaitForSeconds((minReloadTime - maxReloadTime) / 100 * stats.reloadSpeed + maxReloadTime);
            currentMagSize = stats.magSize;
            BroadcastShot(currentMagSize, stats.magSize);
            reloading = false;
        }
    }
}
