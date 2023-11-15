using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishBehaviour : MonoBehaviour
{
    private LevelManager _levelManager;

    private void Start()
    {
        _levelManager = FindObjectOfType<LevelManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (_levelManager != null)
                _levelManager.LoadNextLevel();
            else
                Debug.LogError("There was no levelmanager in the scene! Add one");
        }
    }
}
