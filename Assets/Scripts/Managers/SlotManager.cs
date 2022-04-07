using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    [SerializeField] Transform[] slots;

    int pointer;

    public bool IsFull()
    {
        return pointer == slots.Length;
    }

    public Transform Place()
    {
        return slots[pointer++];
    }

    public void ResetPointer()
    {
        pointer = 0;
    }

    public void Back()
    {
        pointer--;
    }
}
