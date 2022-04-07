using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LiveSystem : MonoBehaviour
{
    public static int maximumLives = 5;
    public static int secondsToGainLive = 1800;

    bool isTicking;

    int seconds;

    [SerializeField] TextMeshProUGUI liveText;
    [SerializeField] TextMeshProUGUI secondsText;

    void Awake()
    {
        CalculateLivesToBeAdded();
        StartCoroutine(Tick());
    }

    void Start()
    {
        Display();
    }

    int GetLives()
    {
        return DataHandler.GetLives();
    }

    void SetLives(int value = -1)
    {
        DataHandler.SetLives(value);
    }

    void LogLastLostLiveTime()
    {
        DataHandler.LogLastLostLiveTime();
    }

    System.DateTime GetLastLostLiveTime()
    {
        return DataHandler.GetLastLostLiveTime();
    }

    bool AreLivesFull()
    {
        return GetLives() == maximumLives;
    }

    public void LostLiveFromMain()
    {
        bool areLivesFull = AreLivesFull();

        SetLives(GetLives() - 1);

        if (areLivesFull)
        {
            isTicking = true;

            StartCountDown();

            StartCoroutine(Tick());

            LogLastLostLiveTime();
        }

        Display();
    }

    public void GainLiveFromMain(bool reset = true)
    {
        SetLives();

        if (AreLivesFull())
        {
            isTicking = false;

            Display();
        }
        else if (reset)
        {
            LogLastLostLiveTime();
            StartCountDown();
        }

        Display();
    }

    public void RefillLives()
    {
        SetLives(maximumLives);
        Display();
    }

    void StartCountDown(int remainingSeconds = -1)
    {
        if (remainingSeconds == -1) seconds = secondsToGainLive;
        else seconds = remainingSeconds;
    }

    void DecreaseSeconds()
    {
        seconds = Mathf.Max(seconds - 1, 0);
    }

    void CheckIsTicking()
    {
        if (GetLives() == maximumLives) isTicking = false;
        else isTicking = true;
    }

    IEnumerator Tick()
    {
        CheckIsTicking();

        while (isTicking)
        {
            Display();

            yield return Extension.Wait.full;

            DecreaseSeconds();

            if (seconds == 0) GainLive();
        }
    }

    void Display()
    {
        liveText.text = GetLives().ToString();

        if (seconds == 0 || AreLivesFull())
        {
            secondsText.text = "Full";
        }
        else
        {
            int minutes = seconds / 60;
            string minutesInString = minutes.ToString().Length > 1 ? minutes.ToString() : "0" + minutes;

            int remainingSeconds = seconds % 60;
            string secondsInString = remainingSeconds.ToString().Length > 1 ? remainingSeconds.ToString() : "0" + remainingSeconds;

            secondsText.text = minutesInString + ":" + secondsInString;
        }
    }

    void CalculateLivesToBeAdded()
    {
        if (AreLivesFull()) return;

        int maximumLivesToBeAdded = maximumLives - GetLives();

        int secondsPassed = (int)(System.DateTime.Now - GetLastLostLiveTime()).TotalSeconds;

        int livesToBeAdded = secondsPassed / secondsToGainLive;

        int remainingSeconds = secondsPassed - (livesToBeAdded * secondsToGainLive);

        if (livesToBeAdded >= maximumLivesToBeAdded)
        {
            RefillLives();
        }
        else
        {
            for(int i = 0; i < livesToBeAdded; i++)
            {
                SetLives();
            }

            StartCountDown(secondsToGainLive - remainingSeconds);
        }
    }

    public static bool CanPlayable()
    {
        return DataHandler.GetLives() > 0;
    }

    public static void LostLive()
    {
        DataHandler.SetLives(DataHandler.GetLives() - 1);
    }

    public static void GainLive()
    {
        DataHandler.SetLives(Mathf.Min(DataHandler.GetLives() + 1, maximumLives));
    }
}
