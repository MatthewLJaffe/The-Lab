using System;
using LabCreationScripts.Spawners;
using UnityEngine;

namespace LabCreationScripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class StretcherController : MonoBehaviour
    {
        [SerializeField] private float changePropertySpeed;
        [SerializeField] private StretcherSpawner.StretcherDirection dir;
        [SerializeField] private PhysicsMaterial2D skidMaterial;
        [SerializeField] private float skidDrag;
        [SerializeField] private float rollDrag;
        [SerializeField] private PhysicsMaterial2D rollMaterial;

        private Rigidbody2D _rb;
     
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (_rb.velocity.magnitude <= changePropertySpeed) {
                _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                return;
            }
            
            if(Mathf.Abs(_rb.velocity.x) > Mathf.Abs(_rb.velocity.y))
            {
                _rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                if (dir == StretcherSpawner.StretcherDirection.Horizontal)
                {
                    _rb.sharedMaterial = rollMaterial;
                    _rb.drag = rollDrag;
                }
                else
                {
                    _rb.sharedMaterial = skidMaterial;
                    _rb.drag = skidDrag;
                }
            }
            else
            {
                _rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                if (dir == StretcherSpawner.StretcherDirection.Horizontal)
                {
                    _rb.sharedMaterial = skidMaterial;
                    _rb.drag = skidDrag;
                }
                else
                {
                    _rb.sharedMaterial = rollMaterial;
                    _rb.drag = rollDrag;
                }
            }
        }
        
    }
}