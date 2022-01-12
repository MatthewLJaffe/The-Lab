using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayerScripts
{
    public class PlayerFind : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        public static PlayerFind Instance;
        public static Action<GameObject> OnPlayerReset = delegate { };
        public static Action PlayerDestroy = delegate {  };
        [HideInInspector] public GameObject playerInstance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                playerInstance = Instantiate(playerPrefab, transform.position, Quaternion.identity);
                DontDestroyOnLoad(playerInstance);
                SceneManager.sceneLoaded += ResetPlayer;
            }
            if (Instance != this)
                Destroy(gameObject);
        }

        public void DestroyPlayer()
        {
            Destroy(playerInstance);
            PlayerDestroy.Invoke();
            playerInstance = null;
            SceneManager.LoadScene(0);
        }
        private void ResetPlayer(Scene scene, LoadSceneMode mode)
        {
            if (playerInstance != null) return;
            playerInstance = Instantiate(playerPrefab, transform.position, Quaternion.identity);
            DontDestroyOnLoad(playerInstance);
            OnPlayerReset.Invoke(playerInstance);
        }
        
    }
}