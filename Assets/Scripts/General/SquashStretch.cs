using System.Collections;
using UnityEngine;

namespace General
{
    public class SquashStretch : MonoBehaviour
    {
        [SerializeField] private AnimationCurve scaleXCurve;
        [SerializeField] private AnimationCurve scaleYCurve;
        [SerializeField] private float duration;
        [SerializeField] private bool queueAnim;
        private bool _playing;

        public void PlayAnimation()
        {
            if (queueAnim || !_playing)
                StartCoroutine(PlaySquashStretch());
        }

        private IEnumerator PlaySquashStretch()
        {
            var scale = transform.localScale;
            if (_playing) {
                yield return new WaitUntil(() => !_playing);
            }
            _playing = true;
            for (var t = 0f; t <= duration; t += Time.deltaTime)
            {
               scale.x = scaleXCurve.Evaluate(t / duration);
               scale.y = scaleYCurve.Evaluate(t / duration);
               transform.localScale = scale;
               yield return null;
            }
            transform.localScale = Vector3.one;
            _playing = false;
        }
        
    }
}