using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataHandler
{
    #region Level

    public static int GetCurrentLevel()
    {
        return PlayerPrefs.GetInt("levelAt", 0);
    }

    public static void SetCurrentLevel(int level)
    {
        PlayerPrefs.SetInt("levelAt", level);
    }

    public static void ExpandLevel()
    {
        PlayerPrefs.SetInt("levelsSolved", GetLevelsSolved() + 1);
    }

    public static int GetLevelsSolved()
    {
        return PlayerPrefs.GetInt("levelsSolved", 0);
    }

    #endregion

    #region Rights

    public static int GetRights(int rightId)
    {
        switch (rightId)
        {
            case 0:
                return PlayerPrefs.GetInt("undo", 0);

            case 1:
                return PlayerPrefs.GetInt("shuffle", 0);

            case 2:
                return PlayerPrefs.GetInt("hint", 0);

            default:
                return -1;
        }
    }

    public static void AddRight(int rightId)
    {
        switch (rightId)
        {
            case 0:
                PlayerPrefs.SetInt("undo", GetRights(0) + 1);
                return;

            case 1:
                PlayerPrefs.SetInt("shuffle", GetRights(1) + 1);
                return;

            case 2:
                PlayerPrefs.SetInt("hint", GetRights(2) + 1);
                return;

            default:
                return;
        }
    }

    public static void RemoveRight(int rightId)
    {
        int rights;
        switch (rightId)
        {
            case 0:
                rights = GetRights(0);
                if (rights == 0) return;
                else
                {
                    PlayerPrefs.SetInt("undo", rights - 1);
                    return;
                }

            case 1:
                rights = GetRights(1);
                if (rights == 0) return;
                else
                {
                    PlayerPrefs.SetInt("shuffle", rights - 1);
                    return;
                }

            case 2:
                rights = GetRights(2);
                if (rights == 0) return;
                else
                {
                    PlayerPrefs.SetInt("hint", rights - 1);
                    return;
                }

            default:
                return;
        }
    }

    #endregion

    #region Seed

    public static int GetSeed(string levelId)
    {
        return PlayerPrefs.GetInt(levelId, 0);
    }

    public static void SetSeed(string levelId, int value = -1)
    {
        if (value == -1) value = 1;

        PlayerPrefs.SetInt(levelId, GetSeed(levelId) + value);
    }

    #endregion

    #region Live

    public static void SetLives(int value = -1)
    {
        if (value == -1) PlayerPrefs.SetInt("lives", Mathf.Min(GetLives() + 1, LiveSystem.maximumLives));
        else PlayerPrefs.SetInt("lives", Mathf.Min(value, LiveSystem.maximumLives));
    }

    public static int GetLives()
    {
        return PlayerPrefs.GetInt("lives", LiveSystem.maximumLives);
    }

    public static void LogLastLostLiveTime()
    {
        PlayerPrefs.SetString("lastLostLiveTime", System.DateTime.Now.ToString());
    }

    public static System.DateTime GetLastLostLiveTime()
    {
        return System.DateTime.Parse(PlayerPrefs.GetString("lastLostLiveTime", "2001-01-03"));
    }

    #endregion

    #region Daily Gift

    public static void LogLastSeizedGiftTime()
    {
        PlayerPrefs.SetString("lastSeizedGiftTime", System.DateTime.Now.ToString());
    }

    public static System.DateTime GetLastSeizedGiftTime()
    {
        return System.DateTime.Parse(PlayerPrefs.GetString("lastSeizedGiftTime", "2001-01-03"));
    }

    public static void SetSeizedGiftPhase(int value)
    {
        PlayerPrefs.SetInt("seizedGiftPhase", value);
    }

    public static int GetSeizedGiftPhase()
    {
        return PlayerPrefs.GetInt("seizedGiftPhase", 0);
    }

    #endregion

    #region Golden Cards

    public static void SetGoldenCards(int value)
    {
        PlayerPrefs.SetInt("goldenCards", value);
    }

    public static int GetGoldenCards()
    {
        return PlayerPrefs.GetInt("goldenCards", 0);
    }

    #endregion

    #region Game Mode

    public static void SetPredefinedGameMode(GameMode mode)
    {
        if (mode == GameMode.Classic) PlayerPrefs.SetInt("predefinedGameMode", 0);
        else PlayerPrefs.SetInt("predefinedGameMode", 1);
    }

    public static GameMode GetPredefinedGameMode()
    {
        if (PlayerPrefs.GetInt("predefinedGameMode", 0) == 0) return GameMode.Classic;
        else return GameMode.Daily;
    }

    public static void SetDailyChallengeLevel(string date)
    {
        PlayerPrefs.SetString("dailyChallengeLevel", date);
    }

    public static string GetDailyChallengeLevel()
    {
        return PlayerPrefs.GetString("dailyChallengeLevel");
    }

    #endregion

    #region Reset

    public static void Reset()
    {
        PlayerPrefs.DeleteAll();
    }

    #endregion

    #region Chest Progresses

    public static int GetLevelChestProgress()
    {
        return PlayerPrefs.GetInt("levelChestProgress", 0);
    }

    public static int GetJourneyChestProgress()
    {
        return PlayerPrefs.GetInt("journeyChestProgress", 0);
    }

    public static void IncrementLevelChestProgress()
    {
        PlayerPrefs.SetInt("levelChestProgress", GetLevelChestProgress() + 1);
    }

    public static void IncrementJourneyChestProgress()
    {
        PlayerPrefs.SetInt("journeyChestProgress", GetJourneyChestProgress() + 1);
    }

    #endregion
}