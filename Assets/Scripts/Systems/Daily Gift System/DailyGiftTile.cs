using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DailyGiftTile : MonoBehaviour
{
    public int value;

    public bool clicked = true;

    public List<DailyGiftTile> blockers;

    public UnityAction<DailyGiftTile> OnPop;

    void Awake()
    {
        for(int i = 0; i < blockers.Count; i++)
        {
            blockers[i].OnPop += Recalibrate;
        }

        if (blockers.Count == 0) clicked = false;
    }

    public void Revaluate(int value)
    {
        this.value = value;
    }

    public void InvokeTiles()
    {
        OnPop?.Invoke(this);
    }

    void Recalibrate(DailyGiftTile tile)
    {
        blockers.Remove(tile);

        if (blockers.Count == 0) clicked = false;
    }
}
