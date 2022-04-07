using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public GameMode gameMode;

    public string levelId;

    public float scale;

    public AnimationType animationType;

    [SerializeField] float alignAmount;

    public bool valuesGiven;

    public List<Tile> tiles { get; private set; }

    public List<int> seeds;

    public bool isTutorial;

    public int countdown;

    public int GetTilesReturnLayerCount()
    {
        tiles = new List<Tile>();

        Tile[] childTiles = GetComponentsInChildren<Tile>();

        int layer = 1;
        for (int i = 0; i < childTiles.Length; i++)
        {
            Tile tile = childTiles[i];
            tiles.Add(tile);

            if ((int)tile.transform.position.z < layer) layer = (int)tile.transform.position.z;
        }

        return layer;
    }

    public float AlignAmount()
    {
        return alignAmount;
    }
}

public enum AnimationType
{
    Random, LayerByLayer, Solitaire, Slide, Assemble, Spread
}

public enum GameMode
{
    Classic, Daily
}