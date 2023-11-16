using UnityEngine;

public class FinishBehaviour : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (LevelManager.Instance != null)
                LevelManager.Instance.LoadNextLevel();
            else
                Debug.LogError("There was no levelmanager in the scene! Add one");
        }
    }
}
