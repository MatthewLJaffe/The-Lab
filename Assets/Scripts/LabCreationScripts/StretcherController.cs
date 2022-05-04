using System;
using System.Collections;
using EntityStatsScripts;
using General;
using LabCreationScripts.Spawners;
using PlayerScripts;
using UnityEngine;

namespace LabCreationScripts
{
    public class StretcherController : MonoBehaviour, IInteractable
    {
        [SerializeField] private StretcherSpawner.StretcherDirection dir;
        [SerializeField] private float maxSpeed;
        [SerializeField] private float rollTime;
        [SerializeField] private AnimationCurve animationCurve;
        [SerializeField] private Color highlightColor;
        [SerializeField] private SoundEffect rollSound;
        [SerializeField] private SpriteRenderer sr;
        [SerializeField] private Rigidbody2D rb;

        private StretcherDamageSource _stretcherDamageSource;

        public bool CanInteract
        {
            set => sr.color = value ? highlightColor : Color.white;
        }

        private Transform _playerTrans;
        private float _prevPlayerSpeed;
     
        private void Awake()
        {
            _stretcherDamageSource = GetComponent<StretcherDamageSource>();
            _stretcherDamageSource.enabled = false;
        }

        private void Start()
        {
            _playerTrans = PlayerFind.instance.playerInstance.transform;
        }

        public void Interact()
        {
            var mag = maxSpeed;
            if (dir == StretcherSpawner.StretcherDirection.Horizontal && _playerTrans.position.x > transform.position.x)
                mag *= -1;
            else if (dir == StretcherSpawner.StretcherDirection.Vertical && _playerTrans.position.y > transform.position.y)
                mag *= -1;
            StartCoroutine(Push(mag));
            
        }

        private IEnumerator Push(float mag)
        {
            rollSound.Play();
            _stretcherDamageSource.enabled = true;
            var waitForFixedUpdate = new WaitForFixedUpdate();
            var pushForce = dir == StretcherSpawner.StretcherDirection.Horizontal ? Vector2.right * mag : Vector2.up * mag;
            for (var t = 0f; t < rollTime; t += Time.fixedDeltaTime) {
                rb.velocity = pushForce * animationCurve.Evaluate(t);
                yield return waitForFixedUpdate;
            }
            _stretcherDamageSource.enabled = false;
            rb.velocity = Vector2.zero;
        }        
    }
}