using PlayerScripts;
using UnityEngine;

namespace WeaponScripts
{
    public class GunRotate : MonoBehaviour
    {
        private SpriteRenderer _sr;
        private Camera _mainCamera;
        private void Awake() {
            _sr = GetComponent<SpriteRenderer>();
            _mainCamera = Camera.main;
        }

        private void FixedUpdate()
        {
            if (!_mainCamera.gameObject.activeSelf) return;
            float theta = Vector2.SignedAngle(_mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.parent.parent.position, Vector2.up);
            RotateGun(theta);
        }

        private void RotateGun(float theta)
        {
            if (theta >= 0)
                transform.rotation = Quaternion.Euler(0, 0, -theta + 90);
            else
                transform.rotation = Quaternion.Euler(0,180, theta + 90);
            _sr.sortingOrder = Mathf.Abs(theta) < 90 ? 1 : 3;
        }
    }
}
