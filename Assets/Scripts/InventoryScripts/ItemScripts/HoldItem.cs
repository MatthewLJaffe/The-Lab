using System;
using UnityEngine;

namespace InventoryScripts.ItemScripts
{
    public class HoldItem : MonoBehaviour
    {
        [SerializeField] private Vector3[] handPositions;
        private SpriteRenderer _sr;
        private Camera _camera;
        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            _camera = Camera.main;
        }

        private void FixedUpdate()
        {
            var angle = Vector2.SignedAngle(Vector2.right, _camera.ScreenToWorldPoint(Input.mousePosition) - transform.parent.parent.position);
            if (angle < 90 && angle > 0)
            {
                transform.localPosition = handPositions[1];
                _sr.sortingOrder = 1;
            }

            if (angle < 180 && angle > 90)
            {
                transform.localPosition= handPositions[2];
                _sr.sortingOrder = 1;

            }

            if (angle < 0 && angle > -90)
            {
                transform.localPosition =  handPositions[0];
                _sr.sortingOrder = 3;

            }
            
            if (angle < -90 && angle > -180)
            {
                transform.localPosition =  handPositions[3];
                _sr.sortingOrder = 3;
            }
        }
    }
}