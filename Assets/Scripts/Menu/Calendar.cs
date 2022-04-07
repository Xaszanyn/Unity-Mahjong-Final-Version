using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class Calendar : MonoBehaviour
{
    [SerializeField] SceneChanger sc;

    [SerializeField] RectTransform monthsRT;

    Month[] months;
    Month currentMonth;
    Button[] currentMonthsButtons;

    [SerializeField] RectTransform ring;

    [SerializeField] Button button;
    [SerializeField] TextMeshProUGUI buttonText;

    [SerializeField] Color yesterdaysColor;
    [SerializeField] Color todaysColor;
    [SerializeField] Color tomorrowsColor;

    bool ad;

    Button todaysChallenge;

    [SerializeField] GameObject leftButton;
    [SerializeField] GameObject rightButton;

    void Awake()
    {
        DisableUnintendedMonths();

        months = monthsRT.GetComponentsInChildren<Month>();

        SetCurrentMonth();
        CheckButtons();
    }

    void DisableUnintendedMonths()
    {
        System.DateTime now = System.DateTime.Now;

        for(int i = 0; i < monthsRT.childCount; i++)
        {
            Month month = monthsRT.GetChild(i).GetComponent<Month>();

            if ((month.year == now.Year && month.month > now.Month) || (month.year > now.Year))
            {
                month.gameObject.SetActive(false);
            }
        }
    }

    void CheckButtons()
    {
        float x = monthsRT.transform.localPosition.x;

        leftButton.SetActive(true);
        rightButton.SetActive(true);

        if (x == 3850)
        {
            leftButton.SetActive(false);
        }
        
        if (x == 3850 - ((months.Length - 1) * 700))
        {
            rightButton.SetActive(false);
        }
    }

    void SetCurrentMonth()
    {
        int month = System.DateTime.Now.Month;

        for(int i = 0; i < months.Length; i++)
        {
            Month selectedMonth = months[i];
            if (month == selectedMonth.month)
            {
                currentMonth = selectedMonth;
                currentMonthsButtons = currentMonth.GetComponentsInChildren<Button>();
            }
        }
    }

    public void GoToMonth(bool direction)
    {
        float x = monthsRT.transform.localPosition.x;

        if (!DOTween.IsTweening(monthsRT))
        {
            monthsRT.DOLocalMoveX(direction ? x - 700 : x + 700, .5F)
                    .SetTarget(monthsRT)
                    .OnComplete(CheckButtons);
        }

        ring.DOAnchorPos(new Vector2(320, 100), 0);
    }

    public void SetButtonText(RectTransform rt)
    {
        string month = rt.parent.parent.parent.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        string day = rt.parent.GetComponent<TextMeshProUGUI>().text;

        DataHandler.SetDailyChallengeLevel(month + " " + day);

        buttonText.text = "Play " + month.Substring(0, 3) + " " + day;
    }

    public void Mark(RectTransform rt)
    {
        ring.DOMove(rt.position, .1F);

        SetButtonText(rt);

        if (todaysChallenge != rt.GetComponent<Button>())
        {
            ad = true;
        }
        else
        {
            ad = false;
        }
    }

    public void CalibrateTheCalendar()
    {
        System.DateTime now = System.DateTime.Now;

        for(int i = 0; i < months.Length; i++)
        {
            Month selectedMonth = months[i];

            bool isFromPast = (selectedMonth.year == now.Year && selectedMonth.month < now.Month) || (selectedMonth.year < now.Year);
            bool isCurrent = selectedMonth == currentMonth;

            Button[] selectedMonthsButtons = selectedMonth.GetComponentsInChildren<Button>();

            for (int j = 0; j < selectedMonthsButtons.Length; j++)
            {
                Button dayButton = selectedMonthsButtons[j];

                TextMeshProUGUI text = dayButton.transform.parent.GetComponent<TextMeshProUGUI>();

                text.text = (j + 1).ToString();

                if (isFromPast)
                {
                    dayButton.interactable = true;

                    text.color = yesterdaysColor;
                }
                else if (isCurrent)
                {
                    if (j + 1 < now.Day)
                    {
                        dayButton.interactable = true;

                        text.color = yesterdaysColor;
                    }
                    else if (j + 1 == now.Day)
                    {
                        dayButton.interactable = true;

                        todaysChallenge = dayButton;
                        text.color = todaysColor;

                        todaysChallenge = dayButton;

                        SetButtonText(dayButton.GetComponent<RectTransform>());
                    }
                    else
                    {
                        dayButton.interactable = false;

                        text.color = tomorrowsColor;
                    }
                }
                else
                {
                    dayButton.interactable = false;

                    text.color = tomorrowsColor;
                }
            }
        }
    }

    public void GoLevel()
    {
        if (ad)
        {
            Debug.Log(">>>>- ADVERTISEMENT -<<<<");
        }

        //dailyLevels.GetLevel(DataHandler.GetDailyChallengeLevel();

        sc.Game(-1);
    }
}