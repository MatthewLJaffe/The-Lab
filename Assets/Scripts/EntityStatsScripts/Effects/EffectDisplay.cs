using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace EntityStatsScripts.Effects
{
    public class EffectDisplay : MonoBehaviour
    {
        [SerializeField] private Effect[] effects;
        [SerializeField] private GameObject displayGameObject;
        [SerializeField] private GameObject displayPrefab;

        private Dictionary<Effect, GameObject> effectVisuals;

        private void Awake()
        {
            Effect.onEffectChange += ChangeVisual;
            effectVisuals = new Dictionary<Effect, GameObject>();
        }

        private void OnEnable()
        {
            StartCoroutine(DisplayEffects());
        }

        private void OnDestroy()
        {
            Effect.onEffectChange -= ChangeVisual;
        }

        private void ChangeVisual(Effect e)
        {
            if (e.Stack < 0) {
                Debug.LogError("Trying to add visual for item that has stack " + e.Stack);
            }
            else if (e.Stack == 0)
            {
                if (effectVisuals.ContainsKey(e))
                {
                    Destroy(effectVisuals[e]);
                    effectVisuals.Remove(e);
                }
            }
            else
            {
                if (!effectVisuals.ContainsKey(e))
                {
                    var visual = Instantiate(displayPrefab, displayGameObject.transform);
                    visual.GetComponent<Image>().sprite = e.sprite;
                    visual.GetComponentsInChildren<TextMeshProUGUI>(true)[1].text = e.message;
                    visual.transform.SetSiblingIndex(0);
                    effectVisuals.Add(e, visual);
                    
                }
                effectVisuals[e].GetComponentsInChildren<TextMeshProUGUI>(true)[0].text = e.Stack > 1 ? "" + e.Stack : "";
            }
        }

        private void RemoveVisual(Effect e)
        {
            if (!effectVisuals.ContainsKey(e)) return;
            if (e.Stack > 0) {
                effectVisuals[e].GetComponentsInChildren<TextMeshProUGUI>()[0].text = 
                    e.Stack > 1 ? "" + e.Stack : "";
            }
            else {
                Destroy(effectVisuals[e]);
                effectVisuals.Remove(e);
            }
        }
        
        private IEnumerator DisplayEffects()
        {
            yield return new WaitForEndOfFrame();
            foreach (var e in effects)
            {
                if (e.Stack == 0 || effectVisuals.ContainsKey(e)) continue;
                var visual = Instantiate(displayPrefab, displayGameObject.transform);
                visual.GetComponent<Image>().sprite = e.sprite;
                visual.GetComponentsInChildren<TextMeshProUGUI>(true)[1].text = e.message;
                var visualInstance = Instantiate(visual, displayGameObject.transform);
                visualInstance.transform.SetSiblingIndex(0);
                effectVisuals.Add(e, visualInstance);
                if (e.Stack > 1)
                    effectVisuals[e].GetComponentsInChildren<TextMeshProUGUI>()[0].text = "" + e.Stack ;
            }
        }
    }
}