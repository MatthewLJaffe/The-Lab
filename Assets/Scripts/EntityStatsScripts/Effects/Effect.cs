using System;
using PlayerScripts;
using UnityEngine;

namespace EntityStatsScripts.Effects
{
    public abstract class Effect : ScriptableObject
    {
        public static  Action<Effect> onEffectChange = delegate{  };
        public Sprite sprite;
        public string message;
        [SerializeField] protected int stack;
        

        public int Stack
        {
            get => stack;
            set
            {
               ChangeEffectStack(value, stack);
               stack = value;
               onEffectChange.Invoke(this);
            }
        }
        
        protected virtual void OnEnable()
        {
            stack = 0;
            PlayerFind.PlayerDestroy += ResetStat;
        }

        protected virtual void OnDisable()
        {
            PlayerFind.PlayerDestroy -= ResetStat;
        }

        protected abstract void ChangeEffectStack(int newStack, int oldStack);
        
        protected void ResetStat()
        {
            stack = 0;
        }
    }
}