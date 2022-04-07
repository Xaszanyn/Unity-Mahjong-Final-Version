using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Menu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelText;

    [SerializeField] SceneChanger sc;

    void Start()
    {
        levelText.text = "Level " + (DataHandler.GetCurrentLevel() + 1);
    }

    public void ContinueGame()
    {
        DataHandler.SetPredefinedGameMode(GameMode.Classic);

        if(LiveSystem.CanPlayable())
        {
            sc.ContinueGame();
        }
        else
        {
            Debug.Log("HOP KARDEÞÝM NEREYE?");
        }
    }
}
