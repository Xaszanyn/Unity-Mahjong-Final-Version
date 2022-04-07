using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] UIManager ui;

    [SerializeField] Sprite tile;
    [SerializeField] Sprite doubleTiles;
    [SerializeField] Sprite tripleTiles;
    [SerializeField] Sprite powerUp;

    [SerializeField] GameManager gm;

    bool doubled;
    int phase;

    public void StartTutorial(string id, bool isTutorial)
    {
        if (!isTutorial) return;

        if (id == "Tutorial 1") ui.StartTutorial(tripleTiles, new Vector2(7.5F, 170), new Vector2(650, 235), new Vector2(-140, -80));
        else if (id == "Tutorial 2") ui.StartTutorial(tripleTiles, new Vector2(7.5F, 170), new Vector2(650, 235), new Vector2(-140, -80));
        else if (id == "Tutorial 3") ui.StartTutorial(doubleTiles, new Vector2(7.5F, 113), new Vector2(650, 235), new Vector2(-140, -80));
        else if (id == "Tutorial 4") ui.StartTutorial(tripleTiles, new Vector2(-137.5F, 168), new Vector2(435, 160), new Vector2(-80, -60));
        else if (id == "Tutorial 5")
        {
            doubled = true;
            phase = 5;
            ui.StartTutorial(tile, new Vector2(7.5F, 124), new Vector2(180, 195), new Vector2(70, -70));
        }
        else if (id == "Tutorial 6")
        {
            doubled = true;
            phase = 6;
            ui.StartTutorial(tile, new Vector2(-360, 165), new Vector2(145, 155), new Vector2(70, -70));
        }
        else if (id == "Tutorial 7")
        {
            phase = 7;
            StartCoroutine(Delay());
        }
        else if (id == "Tutorial 8")
        {
            phase = 8;
            StartCoroutine(Delay());
        }

        IEnumerator Delay()
        {
            yield return Extension.Wait.full;

            ui.StartTutorial(powerUp, new Vector2(285, -800), new Vector2(140, 140), new Vector2(70, -70));
        }
    }

    public void EndTutorial()
    {
        if (doubled)
        {
            if (phase == 5)
            {
                ui.StartTutorial(powerUp, new Vector2(0, -800), new Vector2(140, 140), new Vector2(70, -70), false);
            }
            else if (phase == 6)
            {
                ui.StartTutorial(powerUp, new Vector2(-285, -800), new Vector2(140, 140), new Vector2(70, -70), false);
            }

            doubled = false;

            return;
        }
        else
        {
            if (phase == 5)
            {
                gm.Shuffle();
            }
            else if (phase == 6)
            {
                gm.Undo();
            }
            else if (phase == 7 || phase == 8)
            {
                gm.Hint();
            }
        }

        phase = 0;

        ui.EndTutorial();
    }
}
