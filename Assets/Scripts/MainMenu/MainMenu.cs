using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartNewGame(int gameDifficulty)
    {
        if (gameDifficulty < GameManager.DifficultiesCount)
        {
            GameManager.CurDifficulty = (GameDifficulty) gameDifficulty;
            SceneManager.LoadScene("Game");
            //PlayerPrefs.SetInt("GameDifficulty", gameDifficulty);
        }
    }
}
