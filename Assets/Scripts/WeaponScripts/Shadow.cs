using UnityEngine;

namespace WeaponScripts
{
    public class Shadow : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sr;
        [SerializeField] private Sprite[] shadows;
        [SerializeField] private Transform startHeight;
        [SerializeField] private Transform endHeight;

        public void ShadowProgress(float t)
        {
            transform.position = Vector3.Lerp(startHeight.position, endHeight.position, t);
            sr.sprite = shadows[Mathf.Min(Mathf.FloorToInt(t * shadows.Length), shadows.Length - 1)];
        }
    }
}