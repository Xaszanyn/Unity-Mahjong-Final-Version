using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Classic Levels", menuName = "Scriptable Objects/Classic Levels")]
public class ClassicLevels : ScriptableObject
{
    public List<Level> allLevels;

    public List<Level> templates;
}