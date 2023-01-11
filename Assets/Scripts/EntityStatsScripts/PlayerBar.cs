using System;
using EntityStatsScripts.Effects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace EntityStatsScripts
{
    /// <summary>
    /// used to display player health and infection, originally was meant to be extended for other stats as well
    /// </summary>
    public abstract class PlayerBar : MonoBehaviour
    {
        public static Action<PlayerBarType,float> BarHigh = delegate { };
        public static Action<PlayerBarType, float> BarLow = delegate { };
        public static Action<PlayerBarType> BarDeplete = delegate { };

        public PlayerBarType barType;

        [Tooltip("Can be left uninitialized")]
        [SerializeField] protected float barLowValue;
        [SerializeField] protected float barVeryLowValue;
        [SerializeField] protected float barHighValue;
        [SerializeField] protected float barVeryHighValue;
        [SerializeField] protected GameObject statDisplay;
        [SerializeField] protected RectTransform bar;
        [SerializeField] protected TextMeshProUGUI amountText;
        [SerializeField] protected float minValue;
        [SerializeField] protected float maxValue;
        [SerializeField] protected Effect barHighEffect;
        [SerializeField] protected Effect barLowEffect;
        [SerializeField] protected float maxBarWidth;

        protected float MaxValue
        { 
            get => maxValue;
            set
            {
                float oldWidth = bar.sizeDelta.x;
                bar.sizeDelta = new Vector2(Mathf.Min(oldWidth * value / maxValue, maxBarWidth), bar.sizeDelta.y);
                bar.localPosition += Vector3.right * (bar.sizeDelta.x - oldWidth) / 2;
                var healthDiff = value - MaxValue;
                maxValue = value;
                if (healthDiff + barValue <= 0)
                    BarValue = 1;
                BarValue = Mathf.Min(barValue, maxValue);
                amountText.text = $"{Mathf.Round(barValue)}/{Mathf.Round(maxValue)}";
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
                var newValue = Mathf.Clamp(value, minValue, maxValue);
                var oldValue = Mathf.Clamp(barValue, minValue, maxValue);
                UpdateStatDisplay(newValue / maxValue);
                if (newValue >= barVeryHighValue && oldValue < barVeryHighValue)
                {
                    if (barHighEffect != null)
                        barHighEffect.Stack = 2;
                    if (barLowEffect != null)
                        barLowEffect.Stack = 0;
                }
                else if (newValue >=  barHighValue && newValue < barVeryHighValue 
                        && (oldValue >= barVeryHighValue || oldValue < barHighValue))
                {
                    if (barHighEffect != null)
                        barHighEffect.Stack = 1;
                    if (barLowEffect != null)
                        barLowEffect.Stack = 0;
                }
                else if (newValue <= barVeryLowValue && oldValue > barVeryLowValue)
                {
                    if (barLowEffect != null)
                        barLowEffect.Stack = 2;
                    if (barHighEffect != null)
                        barHighEffect.Stack = 0;
                }
                else if (newValue <= barLowValue && newValue > barVeryLowValue &&
                         (oldValue >= barLowValue || oldValue <= barVeryLowValue))
                {
                    if (barLowEffect != null)
                        barLowEffect.Stack = 1;
                    if (barHighEffect != null)
                        barHighEffect.Stack = 0;
                }
                else if (newValue > barLowValue)
                {
                    if (barLowEffect != null)
                        barLowEffect.Stack = 0;
                    if (barHighEffect != null)
                        barHighEffect.Stack = 0;
                }
                else if (newValue == 0)
                    BarDeplete.Invoke(barType);

                barValue = Mathf.Clamp(value, minValue, maxValue);
                amountText.text = $"{Mathf.Round(barValue)}/{Mathf.Round(maxValue)}";
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
