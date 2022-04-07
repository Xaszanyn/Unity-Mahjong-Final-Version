using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestManager : MonoBehaviour
{
    [SerializeField] List<int> levelChestProgress;
    [SerializeField] List<int> journeyChestProgress;

    public int[] CompleteLevelChestProgress()
    {
        int levelChest = DataHandler.GetLevelChestProgress();

        DataHandler.IncrementLevelChestProgress();

        for (int i = 0; i < levelChestProgress.Count; i++)
        {
            int scale = levelChestProgress[i];

            if (levelChest < scale)
            {
                int[] array = { levelChest, scale };
                return array;
            }
            else
            {
                levelChest -= scale;
            }
        }

        return null;
    }

    public int[] CompleteJourneyChestProgress()
    {
        int journeyChest = DataHandler.GetJourneyChestProgress();

        DataHandler.IncrementJourneyChestProgress();

        for (int i = 0; i < journeyChestProgress.Count; i++)
        {
            int scale = journeyChestProgress[i];

            if (journeyChest < scale)
            {
                int[] array = { journeyChest, scale };
                return array;
            }
            else
            {
                journeyChest -= scale;
            }
        }

        return null;
    }
}
