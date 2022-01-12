using System;
using System.Collections.Generic;
using System.Linq;
using PlayerScripts;
using UnityEngine;

namespace EntityStatsScripts.Effects
{
    [CreateAssetMenu (fileName = "EffectManager", menuName = "")]
    public class EffectManager : ScriptableObject
    {
        /*
        public List<Effect> effects;

        private void OnEnable()
        {
            effects = new List<Effect>();
            PlayerFind.PlayerDestroy += ResetEffects;
        }

        public void AddEffect(Effect e)
        {
            if (effects.Contains(e))
            {
                var effect = effects.Find(entry => entry == e);
                e.stack++;
            }
            effects.Add(e);
            OnAddEffect.Invoke(e);
            e.ApplyEffect();
        }
        
        public void DecrementEffect(Effect e)
        {
            if (!effects.Contains(e)) return;
            e.Stack--;
            if (e.Stack <= 0)
                effects.Remove(e);
            OnRemoveEffect.Invoke(e);
        }
        
        public void RemoveEffect(Effect e)
        {
            if (!effects.Contains(e)) return;
            e.Stack = 0;
            effects.Remove(e);
            OnRemoveEffect.Invoke(e);
        }
        
        //TODO fix 
        public void SetEffectStack(Effect e, int newStack)
        {
            if (newStack <= 0) {
                Debug.LogError("new stack must be positive");
                return;
            }
                
            if (!effects.Contains(e))
                effects.Add(e);
            
            if (newStack > e.stack)
            {
                while (newStack > e.stack)
                {
                    e.stack++;
                    e.ApplyEffect();
                    OnAddEffect.Invoke(e);
                }
            }
            else if (newStack < e.stack)
            {
                while(newStack < e.stack)
                {
                    e.stack--;
                    e.RemoveEffect();
                    OnRemoveEffect.Invoke(e);
                }
            }
        }

        private void ResetEffects()
        {
            effects.Clear();
        }
        */
    }
}