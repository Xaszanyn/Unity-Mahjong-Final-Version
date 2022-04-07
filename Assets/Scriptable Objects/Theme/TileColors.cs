using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tile Colors", menuName = "Scriptable Objects/Tile Colors")]
public class TileColors : ScriptableObject
{
    public Color[] colors = new Color[11];
    public Color[] lockedColors = new Color[11];
}
