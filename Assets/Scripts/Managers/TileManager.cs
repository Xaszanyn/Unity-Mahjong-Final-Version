using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Reflection;

public class TileManager : MonoSingleton<TileManager>
{
    [SerializeField] GameManager gm;
    [SerializeField] UndoManager undo;
    [SerializeField] SlotManager slot;
    [SerializeField] UIManager ui;
    [SerializeField] SumManager sm;
    [SerializeField] LevelLoader ll;

    public float shuffleSpeed;

    List<Tile> tiles;

    List<Tile> selecteds = new List<Tile>();

    [SerializeField] private TileColors tileColors;
    
    List<int> levelNumbers = new List<int>();

    int initialCounter;

    public Color[] testColors;

    public int animationAmount { get; private set; }

    [SerializeField] ParticleSystem particle;

    [SerializeField] TextMeshProUGUI text21;

    List<Tile> goldens = new List<Tile>();

    public int goldensCollected;
    bool goldenMode;

    [SerializeField] TileSprites tileSprites;

    void Start()
    {
        DataHandler.SetGoldenCards(0); // USE IT FOR SETTING GOLDEN MODE

        if (DataHandler.GetGoldenCards() > 0) goldenMode = true;
        else goldenMode = false;

        animationAmount = Enum.GetNames(typeof(AnimationType)).Length;

        ResetInitialCounter();

        goldensCollected = 0;

        GoldenUpdate();
    }

    void GoldenUpdate()
    {
        ui.GoldenCard(goldensCollected, DataHandler.GetGoldenCards(), goldenMode);
    }

    public void Initialize(Level level)
    {
        int layer = level.GetTilesReturnLayerCount();
        tiles = level.tiles;

        CreateLevel(level);

        AlignCenter(level);

        AnimateTiles(layer, level);

        GoldenizeCards();
    }

    void AnimateTiles(int layer, Level level)
    {
        AnimationType animationType = level.animationType;

        if (animationType == 0) animationType = (AnimationType)UnityEngine.Random.Range(1, animationAmount);

        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].Animate(1 - layer, animationType, ll.interval);
        }
    }

    void AlignCenter(Level level)
    {
        float scale = level.scale;
        if (scale <= float.Epsilon) scale = 1;
        level.transform.localScale = new Vector3(scale, scale, level.transform.localScale.z); // .8F, .9F

        level.transform.Centralize(level.AlignAmount());
    }

    private void CreateLevel(Level level)
    {
        if (level.valuesGiven)
        {
            for(int i = 0; i < tiles.Count; i++)
            {
                Tile tile = tiles[i];

                int value = tile.givenValue;

                TileValuate(tile, value);

                tile.TileClick += Clicked;
            }
        }
        else
        {
            if (!ll.random && level.seeds.Count != 0)
            {
                Debug.Log("[ LEVEL ID: " + level.levelId + " ]");

                int seedIndex = DataHandler.GetSeed(level.levelId);

                if (seedIndex < level.seeds.Count)
                {
                    UnityEngine.Random.InitState(level.seeds[seedIndex]);
                    Debug.Log("[ SEED INDEX: " + seedIndex + " ]");
                    Debug.Log("[ SEED: " + level.seeds[seedIndex] + " ]");
                }
                else
                {
                    UnityEngine.Random.InitState(DataHandler.GetCurrentLevel());
                    Debug.Log("SEED >>>>- " + DataHandler.GetCurrentLevel());
                    Debug.Log(">> RANDOM BULLSHIT GO! <<");
                }
            }

            CreateRandoms();

            for (int i = 0; i < tiles.Count; i++)
            {
                Tile tile = tiles[i];

                int value = levelNumbers[i];

                TileValuate(tile, value);

                tile.TileClick += Clicked;
            }

            UnityEngine.Random.InitState(Environment.TickCount);
        }

        void CreateRandoms()
        {
            levelNumbers.Clear();
            int count = 0;
            for (int i = 0; i < tiles.Count; i++)
            {
                int num = UnityEngine.Random.Range(1, 12);
                levelNumbers.Add(num);
                count += num;
            }

            if (count % 21 == 0) return;
            else CreateRandoms();
        }
    }

    public void RearrangeTilesBlocks()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].RearrangeBlocks();
        }
    }

    public void Clicked(Tile tile)
    {
        if (!slot.IsFull() && !tile.isLocked)
        {
            tiles.Remove(tile);
            selecteds.Add(tile);

            undo.AddMove(tile, sm.sum);

            sm.AddSum(tile.value);

            ui.PrintSum(sm.sum);

            TileMoveToSlot(tile, slot.Place().position);

            Check();

            GoldenCheck(tile);
        }

        ui.UpdateUndo();
    }

    void GoldenCheck(Tile tile)
    {
        for (int i = 0; i < goldens.Count; i++)
        {
            Tile golden = goldens[i];

            if (tile == golden)
            {
                goldens.Remove(tile);

                goldensCollected++;
                GoldenUpdate();
            }
            else if (!golden.isLocked)
            {
                Wear(golden);
            }
        }
    }

    void Wear(Tile tile)
    {
        tile.goldenity -= 1;

        Transform piece = tile.transform.GetChild(4).GetChild(0);

        piece.SetAsLastSibling();

        piece.gameObject.SetActive(false);

        if (tile.goldenity == 0)
        {
            tile.golden = false;

            goldens.Remove(tile);
        }
    }

    public void Check(bool fromTile = false)
    {
        if (slot.IsFull()) StartCoroutine(Fail());

        if (sm.sum == 21)
        {
            gm.Hit21(selecteds.Count);

            StartCoroutine(DelayedDisappear(selecteds));

            selecteds.Clear();
            slot.ResetPointer();

            sm.ResetSum();

            if (tiles.Count == 0 && selecteds.Count == 0)
            {
                if(ll.currentLevel.isTutorial && ll.currentLevel.levelId != "Tutorial 8")
                {
                    StartCoroutine(DelayedSuccess());
                    //gm.Success();
                }
                else
                {
                    StartCoroutine(DelayedLevelSuccess());
                    //gm.LevelSuccess();
                }
            }
        }
        else if (sm.sum > 21) gm.Fail();

        if (fromTile)
        {
            ui.UpdateButton(2, true);
        }

        IEnumerator Fail()
        {
            yield return Extension.Wait.end;
            if (slot.IsFull()) gm.Fail();
        }

        IEnumerator DelayedDisappear(List<Tile> selecteds)
        {
            List<Tile> copy = new List<Tile>(selecteds);

            yield return Extension.Wait.half;

            float middle = 0;

            for (int i = 0; i < copy.Count; i++)
            {
                Tile tile = copy[i];

                if (tile == null) continue;

                middle += tile.transform.position.x;
            }

            for (int i = 0; i < copy.Count; i++)
            {
                Tile tile = copy[i];

                if (tile == null) continue;

                tile.Disappear();
                TileDestroy(tile);
            }
        }

        IEnumerator DelayedSuccess()
        {
            yield return Extension.Wait.full;

            gm.LoadNextLevel();
        }

        IEnumerator DelayedLevelSuccess()
        {
            yield return Extension.Wait.full;

            gm.LevelSuccess();
        }
    }

    public void UndoTile(Tile tile, int sum)
    {
        selecteds.Remove(tile);
        tiles.Add(tile);

        sm.SetSum(sum);
        ui.PrintSum(sm.sum);

        slot.Back();
    }

    public void Shuffle()
    {
        if (tiles.Count == 1)
        {
            ui.UpdateButton(1, true);
            return;
        }

        ResetInitialCounter();

        List<Tile> copy = new List<Tile>(tiles);

        for (int i = 0; i < tiles.Count; i++)
        {
            int random;
            while (true)
            {
                random = UnityEngine.Random.Range(0, copy.Count);
                if (i != random) break;
            }

            Tile tile = tiles[i];
            Tile randomTile = copy[random];
            tile.Lock();
            tile.transform.DOMove(randomTile.transform.position, shuffleSpeed)
               .OnComplete(InitialFinisher);

            copy.Remove(randomTile);
        }
    }

    public void InitialFinisher()
    {
        initialCounter++;

        if (initialCounter == tiles.Count)
        {
            ui.UpdateButton(1, true);
            ui.UpdateButton(2, true);

            RearrangeTilesBlocks();
        }
    }

    public void ResetInitialCounter()
    {
        initialCounter = 0;
    }

    public void Hint()
    {
        int needed = 21 - sm.sum;

        var rest = tiles.Where(tile => !tile.isLocked).ToList();
        rest.AddRange(tiles.Where(tile => tile.isLocked).OrderBy(tile => tile.transform.position.z));

        var hints = new List<Tile>();

        int paused = -1;
        for (int i = 0; i < rest.Count; i++)
        {
            Tile tile = rest[i];
            int value = tile.value;

            if (needed - value >= 0)
            {
                hints.Add(tile);
                needed -= value;

                if (needed == 0)
                {
                    if (hints.Count < tiles.Count) ui.UpdateButton(2, true);
                    Hints();
                    return;
                }
            }
            else
            {
                paused = i;
                break;
            }
        }

        Hints();

        if (paused != -1)
        {
            for(int i = paused; i < rest.Count; i++)
            {
                Tile tile = rest[i];

                int value = tile.value;

                int amount;

                if (needed >= value) amount = value;
                else amount = needed;

                if (value - amount == 0)
                {
                    tile.Disappear();
                    TileDestroy(tile);
                }
                else TileValuate(tile, value - amount);

                //Debug.Log(amount);

                sm.AddSum(amount);
                Check(true);

                needed -= amount;

                if (needed == 0) break;
            }
        }
        else
        {
            Debug.LogError("Houston we have a problem!");
            Debug.Break();
        }

        void Hints()
        {
            for (int i = 0; i < hints.Count; i++)
            {
                Tile tile = hints[i];

                tiles.Remove(tile);
                tile.TileClick?.Invoke(tile);
            }
        }
    }

    public void ResetAll()
    {
        slot.ResetPointer();
        selecteds.Clear();

        sm.ResetSum();

        ui.PrintSum();

        goldensCollected = 0;
        goldens.Clear();
    }

    public Color GetColor(int index)
    {
        return tileColors.colors[index - 1];
    }

    public Color GetLockedColor(int index)
    {
        return tileColors.lockedColors[index - 1];
    }

    void GoldenizeCards()
    {
        //DataHandler.SetGoldenCards(6); SETTING

        int goldenAmount = DataHandler.GetGoldenCards();

        List<Tile> copy = new List<Tile>(tiles);

        for(int i = 0; i < goldenAmount; i++)
        {
            Tile tile = copy[UnityEngine.Random.Range(0, copy.Count)];

            goldens.Add(tile);

            Goldenize(tile);

            copy.Remove(tile);
        }

        void Goldenize(Tile tile)
        {
            tile.golden = true;
            tile.goldenity = 4;

            ui.Goldenize(tile);
        }
    }







    void TileMoveToSlot(Tile tile, Vector3 destination)
    {
        tile.LockWithOutFade();
        tile.MoveToSlot(destination);
    }

    void TileDestroy(Tile tile, float delay = .5F)
    {
        Destroy(tile.gameObject, delay);
    }

    void TileValuate(Tile tile, int value)
    {
        tile.value = value;

        tile.Valuate(tileSprites.tiles[value - 1], tileSprites.lockedTiles[value - 1]);
    }
}




// >>>=-=-= LAZIMLIK =-=-=<<< //

//#if UNITY_EDITOR
////public void ClearLog() //you can copy/paste this code to the bottom of your script
////{
////    var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
////    var type = assembly.GetType("UnityEditor.LogEntries");
////    var method = type.GetMethod("Clear");
////    method.Invoke(new object(), null);
////}
//#endif