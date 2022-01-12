using System;
using UnityEngine;
using UnityEngine.Events;

namespace General
{
    public class Toggler : MonoBehaviour
    {
        [Serializable]
        public class ToggleItem
        {
            [SerializeField] private GameObject toToggle;
            [SerializeField] private bool defaultSetting;
            [SerializeField] private UnityEvent OnEnable;
            [SerializeField] private UnityEvent OnDisable;

            public void ToggleTo(bool enable)
            {
                toToggle.SetActive(enable);
                if (enable)
                    OnEnable.Invoke();
                else
                    OnDisable.Invoke();
            }

            public void ApplySetting()
            {
                ToggleTo(defaultSetting);
            }

            public void Toggle()
            {
                ToggleTo(!toToggle.activeSelf);
            }
        }
        [SerializeField] private ToggleItem[] toggledItems;
        [SerializeField] private bool applySettingsOnStart = true;

        private void Start()
        {
            if (applySettingsOnStart)
                ApplySettings();
        }


        public void ApplySettings()
        {
            foreach (var item in toggledItems)
                item.ApplySetting();
        }

        public void ToggleItemsTo(bool enable)
        {
            foreach (var item in toggledItems)
                item.ToggleTo(enable);
        }

        public void ToggleItems()
        {
            foreach (var item in toggledItems)
                item.Toggle();
        }
        
    }
}