using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SumManager : MonoBehaviour
{
    public int sum { get; private set; }

    void Start()
    {
        ResetSum();
    }

    public void AddSum(int value)
    {
        sum += value;
    }

    public void RemoveSum(int value)
    {
        sum -= value;
    }

    public void ResetSum()
    {
        sum = 0;
    }

    public void SetSum(int value)
    {
        sum = value;
    }
}
