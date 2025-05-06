using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerOLD : MonoBehaviour
{
    public static GameManagerOLD instance;
    private int totalPlayers = 1;
    private int deadPlayers = 0;
    public string gameOverSceneName = "GameOverScene";

    //initializes singleton instance and prevents destruction on scene load
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

    //increments dead player count and loads game over scene when threshold reached
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
