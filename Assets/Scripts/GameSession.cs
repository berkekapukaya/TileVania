using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    
    [SerializeField] int playerLives = 3;
    [SerializeField] int coinValue = 100;
    [SerializeField] TextMeshProUGUI livesText;
    [SerializeField] TextMeshProUGUI scoreText;
    
    private int currentScore = 0;
    private int numGameSessions;
    void Awake()
    {
        numGameSessions = FindObjectsOfType<GameSession>().Length;
        Debug.Log("Number of GameSessions: " + numGameSessions);
        if(numGameSessions > 1)
        {
            Debug.Log("Destroying GameSession");
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    
    void Start()
    {
        livesText.text = playerLives.ToString();
        scoreText.text = "0";
    }
    
    public void ProcessPlayerDeath()
    {
        if(playerLives > 1)
        {
            StartCoroutine(TakeLife());
        }
        else
        {
            StartCoroutine(ResetGameSession());
        }
    }
    
    public void ProcessCoinPickup()
    {
        currentScore = int.Parse(scoreText.text);
        currentScore += coinValue;
        scoreText.text = currentScore.ToString();
    }
    
    IEnumerator TakeLife()
    {
        yield return new WaitForSecondsRealtime(1f);
        playerLives--;
        livesText.text = playerLives.ToString();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public IEnumerator ResetGameSession()
    {
        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }
}
