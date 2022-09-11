using System;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class Outline : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private float outlineWidth;
        [SerializeField] private Color outlineColor;

        private void Awake()
        {
            text.outlineWidth = outlineWidth;
            text.outlineColor = outlineColor;
        }
    }
}