using System.Collections;
using EntityStatsScripts.Effects;
using UnityEngine;
using WeaponScripts;

namespace EntityStatsScripts
{
    public class ReloadArmor : MonoBehaviour, IDamageable
    {
        [SerializeField] private ArmoredMagEffect armoredMagEffect;
        private float _armorHealth;
        private Coroutine _armorRoutine;

        private void Awake()
        {
            Gun.broadcastReload += StartEnableArmor;
            gameObject.SetActive(false);
        }

        private void StartEnableArmor(float time)
        {
            if (armoredMagEffect.Stack > 0) {
                gameObject.SetActive(true);
                _armorRoutine = StartCoroutine(EnableArmor(time));
            }
        }
        private IEnumerator EnableArmor(float time)
        {
            _armorHealth = armoredMagEffect.armorHealth;
            yield return new WaitForSeconds(time);
            gameObject.SetActive(false);
        
        }

        public void TakeDamage(float amount, Vector2 dir)
        {
            _armorHealth -= amount;
            if (_armorHealth <= 0) {
                gameObject.SetActive(false);
                StopCoroutine(_armorRoutine);
            }
        }
    }
}
