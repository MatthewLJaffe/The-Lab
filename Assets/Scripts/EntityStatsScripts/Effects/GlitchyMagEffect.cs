using UnityEngine;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu(fileName = "GlitchyMagEffect", menuName = "Effects/GlitchyMagEffect")]
    public class GlitchyMagEffect : Effect
    {
        [SerializeField] private float glitchChancePerStack;
        [SerializeField] private float glitchBaseDamage;
        [SerializeField] private Vector2 damageMultRange;
        public GameObject glitchyBulletPrefab;
        private float _glitchChance;
        
        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            _glitchChance = newStack * glitchChancePerStack;
        }

        public bool IsGlitch()
        {
            if (Stack == 0) return false;
            return Random.Range(0f, 1f) < glitchChancePerStack * Stack;
        }

        public float DetermineDamage(float baseDamage)
        {
            return glitchBaseDamage + Random.Range(damageMultRange.x, damageMultRange.y) * baseDamage;
        }
    }
}