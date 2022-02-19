using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayerScripts
{
    public class PlayerFind : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        public static PlayerFind instance;
        public static Action<GameObject> onPlayerReset = delegate { };
        public static Action playerDestroy = delegate {  };
        [HideInInspector] public GameObject playerInstance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                playerInstance = Instantiate(playerPrefab, transform.position, Quaternion.identity);
                DontDestroyOnLoad(playerInstance);
                SceneManager.sceneLoaded += ResetPlayer;
            }
            if (instance != this)
                Destroy(gameObject);
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= ResetPlayer;
        }

        public void DestroyPlayer()
        {
            Destroy(playerInstance);
            playerDestroy.Invoke();
            playerInstance = null;
            SceneManager.LoadScene(0);
        }
        private void ResetPlayer(Scene scene, LoadSceneMode mode)
        {
            if (playerInstance != null) {
                playerInstance.transform.position = Vector3.zero;
            }
            else
            {
                playerInstance = Instantiate(playerPrefab, transform.position, Quaternion.identity);
                DontDestroyOnLoad(playerInstance);
                onPlayerReset.Invoke(playerInstance);
            }
        }
    }
}