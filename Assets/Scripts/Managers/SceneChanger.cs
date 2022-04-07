using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneChanger : MonoBehaviour
{
    public void Menu()
    {
        SceneManager.LoadScene(0);
    }

    public void Game(int level = -1)
    {
        if (level != -1)
        {
            DataHandler.SetPredefinedGameMode(GameMode.Classic);
            DataHandler.SetCurrentLevel(level);
        }
        else
        {
            DataHandler.SetPredefinedGameMode(GameMode.Daily);
        }

        SceneManager.LoadScene(1);
    }

    public void ContinueGame()
    {
        DataHandler.SetCurrentLevel(DataHandler.GetLevelsSolved());
        SceneManager.LoadScene(1);
    }
}
