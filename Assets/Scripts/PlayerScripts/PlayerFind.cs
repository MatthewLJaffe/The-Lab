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
        public static Action playerInit = delegate {  };
        public static Action playerDestroy = delegate {  };
        [SerializeField] private bool tutorial;
        [HideInInspector] public GameObject playerInstance;

        private void Awake()
        {
            if (tutorial)
            {
                playerInstance = Instantiate(playerPrefab, transform.position, Quaternion.identity);
                instance = this;
                playerInit.Invoke();
                return;
            }
            if (instance == null)
            {
                playerInstance = Instantiate(playerPrefab, transform.position, Quaternion.identity);
                instance = this;
                playerInit.Invoke();
                DontDestroyOnLoad(gameObject);
                DontDestroyOnLoad(playerInstance);
                SceneManager.sceneLoaded += ResetPlayer;
            }
            if (instance != this)
                Destroy(gameObject);
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= ResetPlayer;
            if (instance == this)
                instance = null;
        }

        public void DestroyPlayer()
        {
            Destroy(playerInstance);
            playerDestroy.Invoke();
            playerInstance = null;
            if (tutorial)
                SceneManager.LoadScene(1);
            else
            {
                Destroy(gameObject);
                SceneManager.LoadScene(0);
            }
        }
        private void ResetPlayer(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex == 0)
            {
                if (playerInstance)
                    Destroy(playerInstance);
                Destroy(gameObject);
                return;
            }
            if (playerInstance != null)
            {
                playerInstance.transform.position = transform.position;
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