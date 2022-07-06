using UnityEngine;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu(fileName = "ReverseUnoCardEffect", menuName = "Effects/ReverseUnoCardEffect")]
    public class ReverseUnoCardEffect : Effect
    {
        private float _reverseChance;
        [SerializeField] private float dodgeStep;

        protected override void OnEnable()
        {
            _reverseChance = 0;
            base.OnEnable();
        }

        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            _reverseChance =  (1 - 1f / (dodgeStep * newStack + 1)) * 100;
        }

        public bool RollReverse()
        {
            if (stack == 0)
                return false;
            return Random.Range(0f, 100f) <= _reverseChance;
        }
    }
}