using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Daily Levels", menuName = "Scriptable Objects/Daily Levels")]
public class DailyLevels : ScriptableObject
{
    public List<Month> calendar;

    [System.Serializable] public struct Month
    {
        public string name;
        public List<Level> levels;
    }

    public Level GetLevel(string date)
    {
        string month = date.Split(' ')[0];
        int day = int.Parse(date.Split(' ')[1]);

        for(int i = 0; i < calendar.Count; i++)
        {
            Month matchedMonth = calendar[i];
            if (month == matchedMonth.name)
            {
                for(int j = 0; j < matchedMonth.levels.Count; j++)
                {
                    if (day - 1 == j)
                    {
                        return matchedMonth.levels[j];
                    }
                }
            }
        }

        return null;
    }
}