using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LabCreationScripts
{
    public class RandomSprite : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sr;
        [SerializeField] private Sprite[] sprites;

        private void Awake()
        {
            if (sr == null)
                sr = GetComponent<SpriteRenderer>();
            if (sprites.Length > 0)
                sr.sprite = sprites[Random.Range(0, sprites.Length)];
        }
    }
}