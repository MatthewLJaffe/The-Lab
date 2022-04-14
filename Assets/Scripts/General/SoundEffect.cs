using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace General
{
    [CreateAssetMenu(fileName = "NewSoundEffect", menuName = "Audio/new Sound Effect")]
    public class SoundEffect : ScriptableObject
    {
        [SerializeField] private PlayOrder playOrder;
        public AudioClip[] clips;
        public Vector2 volumeRange = new Vector2(.5f, .5f);
        public Vector2 pitchRange = new Vector2(1f, 1f);
        private int _playIdx;
        private static Type[] _audioPlayerComponents = {typeof(AudioSource)};

        [System.Serializable]
        private enum PlayOrder
        {
            Random,
            Sequential
        }

        #region PreviewSound

#if  UNITY_EDITOR
        private AudioSource _previewer;
        private bool _shouldPlay;



        private void OnDisable()
        {
            if (_previewer)
                DestroyImmediate(_previewer.gameObject);
        }

        [ContextMenu("Play")]
        private async void PlayPreview()
        {
            if (!_previewer)
                _previewer = EditorUtility.CreateGameObjectWithHideFlags(
                        "AudioPreviewer", HideFlags.DontSave, typeof(AudioSource))
                    .GetComponent<AudioSource>();
            _shouldPlay = true;
            await Play(_previewer);
            await Task.Delay(10);
            DestroyImmediate(_previewer.gameObject);
        }
        
        [ContextMenu("Play10")]
        private async void Play10()
        {
            if (!_previewer)
                _previewer = EditorUtility.CreateGameObjectWithHideFlags(
                        "AudioPreviewer", HideFlags.DontSave, typeof(AudioSource))
                    .GetComponent<AudioSource>();
            _shouldPlay = true;
            for (int i = 0; i < 10; i++)
            {
                if (!_shouldPlay) return;
                await Play(_previewer);
                await Task.Delay(10);
            }
            DestroyImmediate(_previewer.gameObject);
        }
        
        [ContextMenu("Stop")]
        private void StopPreview()
        {
            _shouldPlay = false;
            _previewer.Stop();
            DestroyImmediate(_previewer.gameObject);
        }
#endif
        
        #endregion

        public  void PlayInTime(float time, AudioSource audioSource)
        {
            var bestFit = clips[0];
            var diff = Mathf.Abs(time - bestFit.length);
            for (var i= 1; i < clips.Length; i++)
            {
                var currDif = Mathf.Abs(time - clips[i].length);
                if (currDif < diff)
                {
                    bestFit = clips[i];
                    diff = currDif;
                }
            }
            audioSource.pitch = bestFit.length / time;
            audioSource.volume = Random.Range(volumeRange.x, volumeRange.y);
            audioSource.clip = bestFit;
            audioSource.Play();
        }

        public async void Play()
        {
            var audioSource = new GameObject("AudioPlayer", _audioPlayerComponents).GetComponent<AudioSource>();
            await Play(audioSource);
            Destroy(audioSource.gameObject);
        }
        
        public async void Play(GameObject sourceGo)
        {
            var source = sourceGo.GetComponent<AudioSource>();
            if (!source)
                Debug.LogError("No Audio Source found");
            await Play(source);
        }

        public async Task Play(AudioSource audioSourceParam)
        {
            if (clips.Length == 0)
            {
                Debug.LogError($"Missing clips for {name}");
                return;
            }
            
            audioSourceParam.clip = GetClip();
            audioSourceParam.volume = Random.Range(volumeRange.x, volumeRange.y);
            audioSourceParam.pitch = Random.Range(pitchRange.x, pitchRange.y);
            audioSourceParam.Play();
            await Task.Delay((int) (1000 * audioSourceParam.clip.length / audioSourceParam.pitch));
        }

        private AudioClip GetClip()
        {
            switch (playOrder)
            {
                case PlayOrder.Random:
                    return clips[Random.Range(0, clips.Length)];
                case PlayOrder.Sequential:
                    var clip = clips[_playIdx];
                    _playIdx = (_playIdx + 1) % clips.Length;
                    return clip;
                default:
                    return clips[0];
            }
        }
    }
}