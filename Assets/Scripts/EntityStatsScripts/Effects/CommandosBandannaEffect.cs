using PlayerScripts;
using UnityEngine;
using WeaponScripts;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu (fileName = "CommandosBandannaEffect", menuName = "Effects/CommandosBandannaEffect")]
    public class CommandosBandannaEffect : Effect
    {
        private float _damageMult;
        [SerializeField] private float maxDamageMult;
        [SerializeField] private float damageStep;
        [SerializeField] private float maxEffectRange;

        protected override void OnEnable()
        {
            base.OnEnable();
            _damageMult = 0;
            Bullet.BulletDamage += ApplyCommandosBandannaEffect;
        }

        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            _damageMult = maxDamageMult * (1 -  1 / (damageStep * newStack + 1));
            Debug.Log("Stack: " + newStack + " damage mult" + _damageMult);
        }

        private void ApplyCommandosBandannaEffect(Bullet b)
        {
            var distance =
                Vector2.Distance(b.transform.position, PlayerFind.Instance.playerInstance.transform.position);
            if (stack == 0 || b.LayerInMask(LayerMask.NameToLayer("Player")) ||
                distance > maxEffectRange) return;
                b.damage *= _damageMult;
        }
    }
}
