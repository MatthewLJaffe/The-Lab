using System;
using UnityEngine;

namespace EnemyScripts.BeegZombie
{
    public class BeegZombieStunnedState : BaseState
    {
        [SerializeField] private BeegZombieAnimator beegAnimator;
        [SerializeField] private float stunDuration;
        [SerializeField] private BaseState trackState;
        [SerializeField] private float pauseTime;
        private float _currTime;
        public override Type Tick()
        {
            if (_currTime > stunDuration)
                return trackState.GetType();
            if (_currTime > pauseTime)
                beegAnimator.PlayStunnedAnimation();
            _currTime += Time.deltaTime;
            return null;
        }

        protected override void SwitchState(BaseState state)
        {
            if (state == this)
                _currTime = 0;
            base.SwitchState(state);
        }
    }
}