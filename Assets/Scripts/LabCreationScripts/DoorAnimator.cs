using System;
using General;
using UnityEngine;
using UnityEngine.Events;

namespace LabCreationScripts
{
    public class DoorAnimator : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private SoundEffect doorOpen;
        private Direction _doorDir;
        private Animator _animator;
        private bool _doorOpen;
        [SerializeField] private BoxCollider2D doorCollider;
        
        private void Awake()
        {
            _animator = GetComponentInParent<Animator>();
            if (GetComponentInParent<Door>())
                _doorDir = GetComponentInParent<Door>().doorDir;
            if (doorCollider == null)
                doorCollider = transform.parent.GetComponent<BoxCollider2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player") && !_doorOpen && !doorCollider.enabled)
            {
                _doorOpen = true;
                PlayDoorAnimation(other.GetComponent<Rigidbody2D>());
            }
        }
        
        private void PlayDoorAnimation(Rigidbody2D otherRb)
        {
            if (otherRb == null) return;
            
            doorOpen.Play(audioSource);
            if (_doorDir == Direction.Left || _doorDir == Direction.Right)
            {
                if (otherRb.velocity.x > 0)
                    _animator.Play("DoorOpenRight");
                else
                    _animator.Play("DoorOpenLeft");
            }
            else
            {
                if (otherRb.velocity.y > 0)
                    _animator.Play("DoorOpenUp");
                else
                    _animator.Play("DoorOpenDown");
            }
        }

        public void ShowDoorClosed()
        {
            _animator.Rebind();
            _animator.Update(0f);
            _doorOpen = false;
        }
    }
}
