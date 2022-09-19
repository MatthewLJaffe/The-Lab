using System;
using UnityEngine;
using UnityEngine.Events;

namespace PlayerScripts
{
    public class PlayerWin : MonoBehaviour
    {
        public UnityEvent onWin;
        public static Action BroadcastWin = delegate {  };
        
        public void WinGame()
        {
            onWin.Invoke();
            BroadcastWin.Invoke();
        }
    }
}