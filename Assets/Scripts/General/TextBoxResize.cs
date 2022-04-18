using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace General
{
    public class TextBoxResize : MonoBehaviour
    {
        [SerializeField] private float minHeight;
        [SerializeField] private TextMeshProUGUI tmp;
        [SerializeField] private RectTransform resizeRect;
        private Vector2 _newSize;

        private void Awake()
        {
            if (!resizeRect)
                resizeRect = GetComponent<RectTransform>();
            _newSize = resizeRect.sizeDelta;
        }

        private void OnEnable()
        {
            StartCoroutine(Resize());
        }

        private IEnumerator Resize()
        {
            
            if (tmp.text == "") {
                resizeRect.gameObject.SetActive(false);
                yield break;
            }
            yield return new WaitForEndOfFrame();
            _newSize.y = Mathf.Max(minHeight, tmp.textBounds.size.y);
            resizeRect.sizeDelta = _newSize;
        }
    }
}