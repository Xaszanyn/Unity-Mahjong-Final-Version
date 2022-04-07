using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] ClassicLevels classicLevels;
    [SerializeField] DailyLevels dailyLevels;

    [SerializeField] float initialInterval;
    public float interval { get; private set; }

    public bool random;

    void Awake()
    {
        interval = initialInterval;
    }

    public Level currentLevel { get; private set; }

    public GameObject Load(GameMode predefinedGameMode)
    {
        if (predefinedGameMode == GameMode.Classic)
        {
            int currentLevelCount = DataHandler.GetCurrentLevel();
            int allLevelsCount = classicLevels.allLevels.Count;

            if (currentLevelCount >= allLevelsCount)
            {
                UnityEngine.Random.InitState(DataHandler.GetCurrentLevel());

                currentLevel = Instantiate(classicLevels.templates[UnityEngine.Random.Range(0, classicLevels.templates.Count)]);

                UnityEngine.Random.InitState(Environment.TickCount);
            }
            else
            {
                currentLevel = Instantiate(classicLevels.allLevels[DataHandler.GetCurrentLevel()]);
            }

            if (GameManager.gm.tutorialEnabled)
            {
                GameManager.gm.GetUI().ToggleButton(false);

                if (DataHandler.GetCurrentLevel() == 4)
                {
                    GameManager.gm.GetUI().ToggleButton(true, 1);
                }
                else if (DataHandler.GetCurrentLevel() == 5)
                {
                    GameManager.gm.GetUI().ToggleButton(true, 1);
                    GameManager.gm.GetUI().ToggleButton(true, 0);
                }
                else if (DataHandler.GetCurrentLevel() > 5)
                {
                    GameManager.gm.GetUI().ToggleButton(true);
                }
            }
        }
        else
        {
            currentLevel = Instantiate(dailyLevels.GetLevel(DataHandler.GetDailyChallengeLevel()));
        }

        GameManager.gm.GetTileManager().Initialize(currentLevel);

        return currentLevel.gameObject;
    }
}