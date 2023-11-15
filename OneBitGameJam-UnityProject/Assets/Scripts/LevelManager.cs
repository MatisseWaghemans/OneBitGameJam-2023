using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void LoadNextLevel()
    {
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (SceneManager.sceneCountInBuildSettings - 1 == activeSceneIndex)
            SceneManager.LoadScene(0);
        else
            SceneManager.LoadScene(activeSceneIndex + 1);
    }
}
