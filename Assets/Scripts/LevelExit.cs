using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    
    [SerializeField] float levelLoadDelay = 2f;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && SceneManager.GetActiveScene().buildIndex != SceneManager.sceneCountInBuildSettings - 1)
        {
            StartCoroutine(onLevelComplete());
        }
    }
    
    IEnumerator onLevelComplete()
    {
        yield return new WaitForSecondsRealtime(levelLoadDelay);
        
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if(currentSceneIndex == SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        
    }
}
