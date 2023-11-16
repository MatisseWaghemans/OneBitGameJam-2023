using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            ReloadLevel();
    }

    public void LoadNextLevel()
    {
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (SceneManager.sceneCountInBuildSettings - 1 == activeSceneIndex)
            SceneManager.LoadScene(0);
        else
            SceneManager.LoadScene(activeSceneIndex + 1);
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
