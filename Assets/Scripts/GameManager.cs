using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;

    [SerializeField] LevelLoader ll;
    [SerializeField] UndoManager undo;
    [SerializeField] UIManager ui;
    [SerializeField] TileManager tm;
    [SerializeField] Tutorial tut;
    [SerializeField] ChestManager chest;

    public bool isPaused;

    public float comboHardness;

    public GameObject currentLevel;

    public bool tutorialEnabled;

    bool isCountdownIntervened;

    void Awake()
    {
        SetAllConnections();

        Application.targetFrameRate = 60;

        gm = this;

        void SetAllConnections()
        {

        }
    }

    void Start()
    {
        DataHandler.AddRight(0);
        DataHandler.AddRight(0);
        DataHandler.AddRight(1);
        DataHandler.AddRight(1);
        DataHandler.AddRight(2);
        DataHandler.AddRight(2);

        Load(0);
    }

    public UndoManager GetUndo()
    {
        return undo;
    }

    public UIManager GetUI()
    {
        return ui;
    }

    public TileManager GetTileManager()
    {
        return tm;
    }

    void Load(int phase)
    {
        ui.StartLevelComponents();

        isCountdownIntervened = false;

        GameMode predefinedGameMode = DataHandler.GetPredefinedGameMode();

        isPaused = false;

        tm.ResetInitialCounter();
        tm.ResetAll();

        if (phase != 0)
        {
            Destroy(currentLevel);
            undo.ResetMoves();
            ui.UpdateUndo();

            if (phase == 1)
            {
                if (predefinedGameMode == GameMode.Classic)
                {
                    DataHandler.SetSeed(ll.currentLevel.levelId);

                    DataHandler.ExpandLevel();
                    DataHandler.SetCurrentLevel(DataHandler.GetLevelsSolved());
                }
            }
        }

        currentLevel = ll.Load(predefinedGameMode);

        if (tutorialEnabled)
        {
            tut.StartTutorial(ll.currentLevel.levelId, ll.currentLevel.isTutorial);
        }
        
        ui.UpdateButton(1, false);
        ui.UpdateButton(2, false);

        Debug.Log("[ GAME MODE: " + ll.currentLevel.gameMode + " ]");
        if (ll.currentLevel.gameMode == GameMode.Classic) Debug.Log("[ LEVEL: " + (DataHandler.GetCurrentLevel() + 1) + " ]");
        else Debug.Log("[ LEVEL: " + ll.currentLevel.levelId + " ]");

        StartCountdown();
    }

    void StartCountdown()
    {
        if (ll.currentLevel.gameMode != GameMode.Daily || ll.currentLevel.countdown == 0)
        {
            ui.CountdownOff();

            return;
        }

        ui.CountdownOn();

        Coroutine ticking = StartCoroutine(Tick(ll.currentLevel.countdown));

        IEnumerator Tick(int countdown)
        {
            while(countdown > 0)
            {
                if (isCountdownIntervened)
                {
                    break;
                }
                ui.DisplayCountdown(countdown);

                yield return Extension.Wait.full;

                countdown--;
            }

            Debug.Log("ALOOOOOA");
        }
    }

    public void LoadNextLevel()
    {
        Load(1);
    }

    public void Restart()
    {
        Load(2);
    }

    public void Pause()
    {
        isPaused = true;
        ui.Pause();
    }

    public void RestartFromPause()
    {
        ui.ReturnFromPause();
        Restart();
    }

    public void ReturnFromPause()
    {
        ui.ReturnFromPause();
        isPaused = false;
    }

    public void Fail()
    {
        isCountdownIntervened = true;
        LiveSystem.LostLive();

        ui.Fail();
        isPaused = true;
    }

    public void RestartOrTryAgainFromFail()
    {
        ui.ReturnFromFail();
        Restart();
    }

    public void LevelSuccess()
    {
        ui.Success(chest.CompleteLevelChestProgress(), chest.CompleteJourneyChestProgress());
    }

    public void LevelNext()
    {
        ui.ReturnFromSuccess();
        LoadNextLevel();
    }

    public void Hit21(int count)
    {
        ui.Hit21();

        undo.ResetMoves();
    }

    public void Undo()
    {
        if (DataHandler.GetRights(0) == 0)
        {
            Ad(0);
            return;
        }

        undo.Undo();

        DataHandler.RemoveRight(0);

        ui.UpdateUndo();
    }

    public void Shuffle()
    {
        if (DataHandler.GetRights(1) == 0)
        {
            Ad(1);
            return;
        }

        ui.UpdateButton(1, false);
        ui.UpdateButton(2, false);
        tm.Shuffle();

        DataHandler.RemoveRight(1);

        ui.UpdateButtonComponent(1);
    }

    public void Hint()
    {
        StartCoroutine(ui.BlinkShuffle(true));

        if (DataHandler.GetRights(2) == 0)
        {
            Ad(2);
            return;
        }

        ui.UpdateButton(2, false);

        tm.Hint();

        DataHandler.RemoveRight(2);

        ui.UpdateButtonComponent(2);

        tm.Check();
    }

    public void Ad(int rightId)
    {
        ui.FakeAd();

        DataHandler.AddRight(rightId);

        switch(rightId)
        {
            case 0:
                ui.UpdateUndo();
                break;

            case 1:
                StartCoroutine(ui.BlinkShuffle());
                ui.UpdateButtonComponent(1);
                break;

            case 2:
                ui.UpdateButtonComponent(2);
                break;
        }
    }
}