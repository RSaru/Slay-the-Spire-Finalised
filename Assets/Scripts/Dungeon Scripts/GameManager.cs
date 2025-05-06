using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private int totalPlayers = 1;
    private int deadPlayers = 0;
    public string gameOverSceneName = "GameOverScene";

    //initializes singleton instance and prevents destruction on load
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //increments dead player count and checks for game over
    public void PlayerDied()
    {
        deadPlayers++;
        if (deadPlayers >= totalPlayers)
        {
            SceneManager.LoadScene(gameOverSceneName);
        }
    }

    //resets the dead player count
    public void ResetPlayerCount()
    {
        deadPlayers = 0;
    }
}
