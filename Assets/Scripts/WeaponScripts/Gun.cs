using System;
using System.Collections;
using EntityStatsScripts;
using EntityStatsScripts.Effects;
using General;
using PlayerScripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WeaponScripts
{
    public class Gun : MonoBehaviour
    {
        public static Action<int, int> broadcastShot = delegate { };
        public static Action<float> broadcastReload = delegate { };
        public static Action<Gun> broadCastWeaponSwitch = delegate {  };
        [SerializeField] protected PlayerStats playerStats;
        [SerializeField] protected GameObject bullet;
        [SerializeField] private float shake;
        public GunStats gunStats;
        [SerializeField] protected Transform shootPoint;
        private bool _firstEquip = true;
        protected Transform playerTrans;
        protected Camera mainCamera;

        public int CurrentMagSize
        {
            get => currentMagSize;
            set
            {
                currentMagSize = value;
                broadcastShot(currentMagSize, gunStats.magSize);
            }
        }
        protected int currentMagSize;
        protected bool reloading;
        protected bool firing;
        protected static float maxReloadTime = 3;
        protected static float minReloadTime = .1f;
        protected GameObjectPool _bulletPool;
        protected float atkMult;
        protected float playerCritChance;
        protected float additionalAccuracy;
        protected float additionalFireRate;
        protected float reloadFactor;
        
        protected void Start() 
        {
            mainCamera = Camera.main;
            currentMagSize = gunStats.magSize;
            broadcastShot(currentMagSize, gunStats.magSize);
            _firstEquip = false;
            _bulletPool = new GameObjectPool(bullet);
            playerTrans = PlayerFind.instance.playerInstance.transform;
            atkMult = playerStats.GetAttackMultiplier();
            playerCritChance = playerStats.PlayerStatsDict[PlayerStats.StatType.CritChance].CurrentValue;
            additionalAccuracy = playerStats.PlayerStatsDict[PlayerStats.StatType.Accuracy].CurrentValue;
            additionalFireRate = playerStats.PlayerStatsDict[PlayerStats.StatType.FireRate].CurrentValue;
            reloadFactor = playerStats.PlayerStatsDict[PlayerStats.StatType.ReloadFactor].CurrentValue;
            PlayerStats.OnStatChange += delegate(PlayerStats.StatType type, float newValue) 
            {
                switch (type)
                {
                    case PlayerStats.StatType.Attack:
                        atkMult = playerStats.GetAttackMultiplier();
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
                    case PlayerStats.StatType.ReloadFactor:
                        reloadFactor = newValue;
                        break;
                }
            };
        }

        protected void OnEnable() {
            if (!_firstEquip)
                broadcastShot(currentMagSize, gunStats.magSize);
            PlayerInputManager.OnInputDown += StartReload;
            broadCastWeaponSwitch.Invoke(this);
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

        private IEnumerator Fire()
        {
            if (reloading || firing)
                yield break;
            currentMagSize--;
            CameraShakeController.invokeShake(shake);
            broadcastShot(currentMagSize, gunStats.magSize);
            ShootProjectile();
            if (currentMagSize == 0)
                StartCoroutine(Reload());
            firing = true;
            yield return new WaitForSeconds(60 / (gunStats.fireRate + gunStats.fireRate * additionalFireRate / 10));
            firing = false;
        }

        protected virtual void ShootProjectile()
        {
            if (!mainCamera.gameObject.activeSelf) return;
            var bulletInstance = _bulletPool.GetFromPool();
            bulletInstance.transform.position = shootPoint.position;
            var bulletComponent = bulletInstance.GetComponent<PooledBullet>();
            var mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            bulletComponent.damage = Mathf.Max(1f, gunStats.damage * atkMult);
            bulletComponent.crit = gunStats.critChance + playerCritChance > Random.Range(0f, 100f);
            bulletComponent.accuracy = Mathf.Clamp(gunStats.accuracy + additionalAccuracy, 0f, 100f);
            bulletComponent.speed = bulletComponent.accuracy / 10f + 2;
            if (Vector2.Distance(mousePos, playerTrans.position) > 1)
                bulletComponent.direction = mousePos - shootPoint.position;
            else
                bulletComponent.direction = mousePos - playerTrans.position;
        }
        
        protected IEnumerator Reload()
        {
            if (reloading || firing || currentMagSize == gunStats.magSize)
                yield break;
            reloading = true;
            var reloadTime = ((minReloadTime - maxReloadTime) / 100 * gunStats.reloadSpeed + maxReloadTime) * reloadFactor;
            broadcastReload.Invoke(reloadTime);
            yield return new WaitForSeconds(reloadTime);
            currentMagSize = gunStats.magSize;
            broadcastShot(currentMagSize, gunStats.magSize);
            reloading = false;
        }
    }
}
