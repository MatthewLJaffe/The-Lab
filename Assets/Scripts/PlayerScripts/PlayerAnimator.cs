using UnityEngine;

namespace PlayerScripts
{
    public class PlayerAnimator : MonoBehaviour
    {
        private Animator _anim;
        private Rigidbody2D _rb;
        [SerializeField] private OrientationData[] orientationData;
        private Camera _mainCamera;


        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody2D>();
            _mainCamera = Camera.main;
        }
        [System.Serializable]
        private struct OrientationData 
        {
            public int direction;
            public bool[] isForward;
        }


        private void Update() {
            SetAnimation();
        }
        private void SetAnimation()
        {
            var lookDir = CalculateLookDirection();
            var animation = "IdleDown";
            switch (lookDir)
            {
                case 1:
                    animation = "IdleUpRight";
                    break;
                case 3:
                    animation = "IdleDownRight";
                    break;
                case 5:
                    animation = "IdleDownLeft";
                    break;
                default:
                    animation = "IdleUpLeft";
                    break;
            }
            
            if (_rb.velocity.magnitude > .1f) {
                animation = animation.Replace("Idle", "Run");
                foreach (var o in orientationData)
                {
                    if (o.direction != lookDir) continue;
                    var dir = CalculateDirection();
                    if (!o.isForward[dir])
                        animation = animation.Replace("Run", "Reverse");
                }
            }

            _anim.Play(animation);
        }

        private int CalculateLookDirection()
        {
            if (!_mainCamera.gameObject.activeSelf) return 1;
            float theta = AngleFromPoints(_mainCamera.ScreenToWorldPoint(Input.mousePosition), transform.position);
            if (theta > 0 && theta <= 90)
                return 1;
            if (theta > 90 && theta <= 180)
                return 3;
            if (theta > 180 && theta <= 270)
                return 5;
            if (theta > 270 && theta <= 360)
                return 7;
            return 1;
        }

        private int CalculateDirection()
        {
            float theta = AngleFromPoints(_rb.velocity, Vector2.zero);
            if (theta < 0)
                theta += 360;
            if (theta > 22.5f && theta <= 67.5f)
                return 1;
            if (theta > 67.5 && theta <= 112.5f)
                return 2;
            if (theta > 112.5f && theta <= 157.5f)
                return 3;
            if (theta > 157.5 && theta <= 202.5f)
                return 4;
            if (theta > 202.5f && theta <= 247.5)
                return 5;
            if (theta > 247.5f && theta <= 292.5f)
                return 6;
            if (theta > 292.5 && theta <= 327.5f)
                return 7;
            return 0;
        }

        /// <summary>
        /// returns point 1s angle from point 2 in positive degrees
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static float AngleFromPoints(Vector2 point1, Vector2 point2)
        {
            var dir = point1 - point2;
            float theta = Mathf.Rad2Deg * Mathf.Atan2(dir.x, dir.y);
            if (theta < 0)
                theta += 360;
            return theta;

        }
    }
}
