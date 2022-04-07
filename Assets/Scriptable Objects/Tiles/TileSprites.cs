using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tile Sprites", menuName = "Scriptable Objects/Tile Sprites")]
public class TileSprites : ScriptableObject
{
    public Sprite[] tiles = new Sprite[11];
    public Sprite[] lockedTiles = new Sprite[11];
}