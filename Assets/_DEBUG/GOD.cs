using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOD : MonoBehaviour
{
    public bool isMain;

    [SerializeField] LiveSystem ls;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GameManager.gm.LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            ResetData();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            DataHandler.AddRight(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DataHandler.AddRight(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            DataHandler.AddRight(2);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            GameManager.gm.Undo();
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            GameManager.gm.Shuffle();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            GameManager.gm.Hint();
        }
        else if (isMain && Input.GetKeyDown(KeyCode.R))
        {
            ls.RefillLives();
        }
        else if (isMain && Input.GetKeyDown(KeyCode.T))
        {
            ls.GainLiveFromMain(false);
        }
        else if (isMain && Input.GetKeyDown(KeyCode.Y))
        {
            ls.LostLiveFromMain();
        }
        //else if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Debug.Log(CompleteLevelChestProgress());
        //    Debug.Log(CompleteJourneyChestProgress());
        //}
        else if (Input.GetKeyDown(KeyCode.M))
        {
            PlayerPrefs.SetInt("journeyChestProgress", 0);
            PlayerPrefs.SetInt("levelChestProgress", 0);
        }
    }

    public void ResetData()
    {
        DataHandler.Reset();
    }

    public void GivePowerUps()
    {
        for(int i = 0; i < 25; i++)
        {
            DataHandler.AddRight(0);
            DataHandler.AddRight(1);
            DataHandler.AddRight(2);
        }
    }

    public void SkipTutorial()
    {
        DataHandler.SetCurrentLevel(8);
        Debug.Log(DataHandler.GetLevelsSolved());
    }
}
