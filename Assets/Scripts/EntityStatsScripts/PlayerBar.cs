using System;
using EntityStatsScripts.Effects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace EntityStatsScripts
{
    public abstract class PlayerBar : MonoBehaviour
    {
        public static Action<PlayerBarType,float> BarHigh = delegate { };
        public static Action<PlayerBarType, float> BarLow = delegate { };
        public static Action<PlayerBarType> BarDeplete = delegate { };

        public PlayerBarType barType;
        [Tooltip("Can be left uninitialized")]
        [SerializeField] protected GameObject statDisplay;
        [SerializeField] protected RectTransform bar;
        [SerializeField] protected TextMeshProUGUI amountText;
        [SerializeField] protected float minValue;
        [SerializeField] protected float maxValue;
        [SerializeField] protected Effect barHighEffect;
        [SerializeField] protected Effect barLowEffect;

        protected float MaxValue
        {
            get => maxValue;
            set
            {
                float oldWidth = bar.sizeDelta.x;
                bar.sizeDelta = new Vector2(oldWidth * value / maxValue, bar.sizeDelta.y);
                bar.localPosition += Vector3.right * (bar.sizeDelta.x - oldWidth) / 2;
                var healthDiff = value - MaxValue;
                maxValue = value;
                if (healthDiff + barValue <= 0)
                    BarValue = 1;
                else
                    BarValue += healthDiff;
            }
        }
        
        [SerializeField] protected float defaultValue;
        [Tooltip("Initialized at runtime")]
        [SerializeField] protected float barValue;

        public virtual float BarValue
        {
            get => barValue;
            set
            {
                var newPercentage = Mathf.Clamp(value / maxValue, 0, 1);
                var oldPercentage = Mathf.Clamp(barValue / maxValue, 0, 1);
                UpdateStatDisplay(newPercentage);
                if (newPercentage >= .75f && oldPercentage < .75f)
                {
                    barHighEffect.Stack = 2;
                    barLowEffect.Stack = 0;
                }
                else if (newPercentage >= .5f && newPercentage < .75f && (oldPercentage >= .75f || oldPercentage < .5f))
                {
                    barHighEffect.Stack = 1;
                    barLowEffect.Stack = 0;
                }
                    
                else if (newPercentage <= .25f && oldPercentage > .25f)
                {
                    barLowEffect.Stack = 2;
                    barHighEffect.Stack = 0;
                }
                else if (newPercentage <= .5f && newPercentage > .25f &&
                         (oldPercentage >= .5f || oldPercentage <= .25f))
                {
                    barLowEffect.Stack = 1;
                    barHighEffect.Stack = 0;
                }
                else if (newPercentage == 0)
                    BarDeplete.Invoke(barType);
                barValue = Mathf.Clamp(value, minValue, maxValue);
                amountText.text = $"{Mathf.Round(barValue)}/{maxValue}";
            }
        }

        public enum PlayerBarType
        {
            Health,
            Infection
        }

        protected virtual void UpdateStatDisplay(float percentage)
        {
            if (statDisplay != null && statDisplay.GetComponentInChildren<Slider>() != null)
                statDisplay.GetComponentInChildren<Slider>().value = percentage;
        }

        public virtual void Initialize()
        {
            BarValue = defaultValue;
        }
    }
}
