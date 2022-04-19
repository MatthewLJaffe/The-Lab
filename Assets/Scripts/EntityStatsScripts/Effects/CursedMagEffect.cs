using System.Threading.Tasks;
using PlayerScripts;
using UnityEngine;
using WeaponScripts;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu (fileName = "CursedMagEffect", menuName = "Effects/CursedMagEffect")]
    public class CursedMagEffect : Effect
    {
        [SerializeField] private float damageChance;
        [SerializeField] private float damageBuffTime;
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private float minReloadFactor;
        [SerializeField] private float attackStep;
        [SerializeField] private float reloadStep;
        [SerializeField] private float damagePerStack;
        private float _damage;
        private float _attackBonus;
        
        protected override  void OnEnable()
        {
            base.OnEnable();
            Gun.broadcastReload += ApplyCursedMagEffect;
        }
        
        protected override  void OnDisable()
        {
            base.OnDisable();
            Gun.broadcastReload -= ApplyCursedMagEffect;
        }

        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            //undo effect of old stack
            playerStats.playerStatsDict[PlayerStats.StatType.ReloadFactor].CurrentValue /=
                minReloadFactor + (1f - minReloadFactor) / (reloadStep + oldStack);

            //apply effect of new stack
            playerStats.playerStatsDict[PlayerStats.StatType.ReloadFactor].CurrentValue *=
                minReloadFactor +  (1f - minReloadFactor) / (reloadStep + newStack);
            _attackBonus = attackStep * newStack;
            _damage = newStack * damagePerStack;
        }

        public async void ApplyCursedMagEffect(float reloadTime)
        {
            if (stack == 0) return;
            
            if (Random.Range(0, 1f) < damageChance) {
                DamagePlayer.applyPlayerDamage(_damage, Vector2.zero);
            }

            playerStats.playerStatsDict[PlayerStats.StatType.Attack].CurrentValue += _attackBonus;
            var end = Time.time + damageBuffTime + reloadTime;
            while (end > Time.time)
                await Task.Yield();
            playerStats.playerStatsDict[PlayerStats.StatType.Attack].CurrentValue -= _attackBonus;
        }
    }
}