using UnityEngine;
using UnityEngine.Events;

namespace Tutorial
{
    public class TutorialDoor : MonoBehaviour
    {
        public UnityEvent onRoomEnter;
        
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                onRoomEnter.Invoke();
            }
        }
        
    }
}