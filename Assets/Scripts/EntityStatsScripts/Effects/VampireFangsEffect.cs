using UnityEngine;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu(fileName = "VampireFangs", menuName = "Effects/VampireFangs")]
    public class VampireFangsEffect : Effect
    {
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private float[] hpReductions;
        [SerializeField] private Vector2 healRange;
        [SerializeField] private Vector2 probRange;
        [SerializeField] private Vector2 damageRange;
        private float _hpReduction;

        protected override void OnEnable()
        {
            _hpReduction = 0;
            base.OnEnable();
        }

        public void RollLifesteal(float damage)
        {
            if (stack == 0) return;
            var lerpPoint = (damage - damageRange.x) / (damageRange.y - damageRange.x);
            var prob = Mathf.Lerp(probRange.x, probRange.y, lerpPoint) * stack;
            if (Random.Range(0f, 1f) > prob) return;
            var heal = Mathf.Lerp(healRange.x, healRange.y, lerpPoint) * stack;
            PlayerBarsManager.Instance.ModifyAndDisplayStat(PlayerBar.PlayerBarType.Health, heal);
        }
        protected override void ChangeEffectStack(int newStack, int oldStack)
        {
            var newReduction = newStack - 1 < hpReductions.Length ? hpReductions[newStack - 1] : hpReductions[hpReductions.Length - 1];
            playerStats.playerStatsDict[PlayerStats.StatType.MaxHealth].CurrentValue += newReduction - _hpReduction;
            _hpReduction = newReduction;
        }
    }
}