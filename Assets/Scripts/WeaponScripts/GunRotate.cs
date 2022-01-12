using PlayerScripts;
using UnityEngine;

namespace WeaponScripts
{
    public class GunRotate : MonoBehaviour
    {
        private SpriteRenderer sr;
        private void Awake() {
            sr = GetComponent<SpriteRenderer>();
        }

        private void FixedUpdate()
        {
            float theta = Vector2.SignedAngle(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.parent.parent.position, Vector2.up);
            RotateGun(theta);
        }

        private void RotateGun(float theta)
        {
            if (theta >= 0)
                transform.rotation = Quaternion.Euler(0, 0, -theta + 90);
            else
                transform.rotation = Quaternion.Euler(0,180, theta + 90);
            sr.sortingOrder = Mathf.Abs(theta) < 90 ? 1 : 3;
        }
    }
}
