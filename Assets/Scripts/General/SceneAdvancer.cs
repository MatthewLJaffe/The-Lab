using PlayerScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace General
{
    public class SceneAdvancer : MonoBehaviour
    {
        public void NextScene()
        {
            if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            else
                Debug.LogError("Trying to load out of bounds scene");
        }
        
        public void LoadScene(int buildIdx)
        {
            if (buildIdx < SceneManager.sceneCountInBuildSettings && buildIdx >= 0)
            {
                if (buildIdx <= 2 && PlayerFind.instance)
                    PlayerFind.instance.DestroyPlayer();
                SceneManager.LoadScene(buildIdx);
            }
            else
                Debug.LogError("Trying to load out of bounds scene");
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}