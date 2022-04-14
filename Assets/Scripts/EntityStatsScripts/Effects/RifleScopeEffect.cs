using PlayerScripts;
using UnityEngine;
using WeaponScripts;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu (fileName = "RifleScopeEffect", menuName = "Effects/RifleScopeEffect")]
    public class RifleScopeEffect : Effect
    {
        private float _critChanceBonus;
        [SerializeField] private float critStep;
        [SerializeField] private float minEffectRange;

        protected override void OnEnable()
        {
            base.OnEnable();
            _critChanceBonus = 0;
            PlayerBullet.bulletDamage += ApplyRifleScopeEffect;
        }

        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            _critChanceBonus = Mathf.Min(1, critStep * newStack);
        }

        private void ApplyRifleScopeEffect(PlayerBullet b)
        {
            var distance =
                Vector2.Distance(b.transform.position, PlayerFind.instance.playerInstance.transform.position);
            if (stack == 0 || b.crit || b.LayerInMask(LayerMask.NameToLayer("Player")) ||
                distance < minEffectRange) return;
            if ( Random.Range(0f, 1f)< _critChanceBonus) {
                b.damage *= 2.5f;
                b.crit = true;
            }
        }
    }
}
