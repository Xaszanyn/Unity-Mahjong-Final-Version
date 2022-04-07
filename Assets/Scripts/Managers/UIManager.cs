using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Essentials")]
    [SerializeField] SumManager sm;
    [SerializeField] UndoManager undo;
    [SerializeField] TileManager tm;

    [Header("Panels")]
    [SerializeField] Image fade;
    [SerializeField] RectTransform pause;
    [SerializeField] RectTransform success;
    [SerializeField] RectTransform fail;
    [SerializeField] RectTransform successText;
    [SerializeField] CanvasGroup reward;
    [SerializeField] CanvasGroup levelChestProgress;
    [SerializeField] Image levelChestProgressBar;
    [SerializeField] TextMeshProUGUI levelChestProgressAmount;
    [SerializeField] CanvasGroup journeyChestProgress;
    [SerializeField] Image journeyChestProgressBar;
    [SerializeField] TextMeshProUGUI journeyChestProgressAmount;
    [SerializeField] RectTransform continueButton;
    [SerializeField] RectTransform homeButton;
    [SerializeField] CanvasGroup failCG;
    [SerializeField] RectTransform failText;
    [SerializeField] TextMeshProUGUI holdText;
    [SerializeField] RectTransform tryAgainButton;
    [SerializeField] RectTransform giveUpButton;

    [Header("Sum")]
    [SerializeField] Image sumFrame;
    [SerializeField] RectTransform sumFrameRT;
    [SerializeField] TextMeshProUGUI sumText;
    [SerializeField] RectTransform sumTextRT;

    [Header("Power-ups")]
    [SerializeField] Button undoButton;
    [SerializeField] GameObject undoRightsCircle;
    [SerializeField] TextMeshProUGUI undoRightsText;
    [SerializeField] Button shuffleButton;
    [SerializeField] GameObject shuffleRightsCircle;
    [SerializeField] TextMeshProUGUI shuffleRightsText;
    [SerializeField] Button hintButton;
    [SerializeField] GameObject hintRightsCircle;
    [SerializeField] TextMeshProUGUI hintRightsText;

    [Header("Tutorial")]
    [SerializeField] RectTransform spot;
    [SerializeField] CutoutMask spotMask;
    [SerializeField] RectTransform pointer;
    [SerializeField] Image pointerImage;
    [SerializeField] Color spotColor;
    Sequence pointerSeq;

    [Header("Components")]
    [SerializeField] RectTransform powerUps;
    [SerializeField] SpriteRenderer slotsSprite;
    [SerializeField] CanvasGroup levelComponents;

    [Header("Others")]
    [SerializeField] TextMeshProUGUI countdownText;
    [SerializeField] GameObject goldenBoard;
    [SerializeField] TextMeshProUGUI goldenText;

    [Header("Unfinished")]
    [SerializeField] GameObject fakeAd;
    [SerializeField] ParticleSystem confettiLeft;
    [SerializeField] ParticleSystem confettiRight;

    void Start()
    {
        PrintSum();

        UpdateUndo();

        UpdateButton(1, false);
        UpdateButtonComponent(1);

        UpdateButton(2, false);
        UpdateButtonComponent(2);

        pointerSeq = DOTween.Sequence();
        pointerSeq.Append(pointer.DOScale(Vector2.one * 1.2F, .5F));
        pointerSeq.Append(pointer.DOScale(Vector2.one, .5F));
        pointerSeq.SetLoops(-1);
    }

    public void Fail()
    {
        FadeOn();

        fail.gameObject.SetActive(true);

        failCG.DOFade(1, .5F);

        failText.localPosition = new Vector2(0, 900);
        tryAgainButton.localPosition = new Vector2(-820, -1000);
        giveUpButton.localPosition = new Vector2(760, -1000);
        Color color = holdText.color;
        color.a = 0;
        holdText.color = color;

        failText.DOLocalMoveY(240, .5F).SetDelay(.5F);
        tryAgainButton.DOLocalMoveX(-260, .5F).SetDelay(.5F);
        giveUpButton.DOLocalMoveX(320, .5F).SetDelay(.5F);
        holdText.DOFade(1, .5F).SetDelay(.5F);
    }

    public void Pause()
    {
        FadeOn();

        pause.gameObject.SetActive(true);
        pause.localScale = Vector3.zero;
        pause.DOScale(Vector3.one, .4F)
            .SetDelay(.25F)
            .SetEase(Ease.OutBack);
    }

    public void Success(int[] levelChest, int[] journeyChest, bool withContinue = false)
    {
        levelChestProgressAmount.text = levelChest[0] + "/" + levelChest[1];
        journeyChestProgressAmount.text = journeyChest[0] + "/" + journeyChest[1];

        EndLevelComponents();

        confettiLeft.Play();
        confettiRight.Play();

        FadeOn();

        success.gameObject.SetActive(true);

        successText.localScale = Vector2.zero;
        reward.alpha = 0;
        levelChestProgress.alpha = 0;
        journeyChestProgress.alpha = 0;
        continueButton.localScale = Vector2.zero;
        homeButton.localScale = Vector2.zero;

        successText.DOScale(Vector2.one, .5F).SetDelay(.5F);
        reward.DOFade(1, .5F).SetDelay(1)
            .OnComplete(() =>
            {
                levelChestProgressAmount.text = (levelChest[0] + 1) + "/" + levelChest[1];
                journeyChestProgressAmount.text = (journeyChest[0] + 1) + "/" + journeyChest[1];
            });

        float fromLevel = levelChest[0] / (float)levelChest[1];
        float toLevel = (levelChest[0] + 1) / (float)levelChest[1];

        float fromJourney = journeyChest[0] / (float)journeyChest[1];
        float toJourney = (journeyChest[0] + 1) / (float)journeyChest[1];

        levelChestProgress.DOFade(1, .5F).SetDelay(1.5F);
        levelChestProgressBar.fillAmount = fromLevel;
        IncrementProgress(levelChestProgressBar, fromLevel, toLevel, 1.75F);

        journeyChestProgress.DOFade(1, .5F).SetDelay(1.75F);
        journeyChestProgressBar.fillAmount = fromJourney;
        IncrementProgress(journeyChestProgressBar, fromJourney, toJourney, 2);

        if (withContinue) continueButton.DOScale(1, .5F).SetDelay(2.5F).OnComplete(() => homeButton.DOScale(1, .5F));
        else homeButton.DOScale(1, .5F).SetDelay(2.5F);
    }

    void IncrementProgress(Image progress, float from, float to, float delay)
    {
        DOTween.To(() => from, x => progress.fillAmount = x, to, .5F).SetDelay(delay);
    }

    public void ReturnFromSuccess()
    {
        FadeOff();

        if (!DOTween.IsTweening(pause))
        {
            success.DOScale(Vector3.zero, .4F)
                .SetEase(Ease.InBack)
                .OnComplete(() => pause.gameObject.SetActive(false));
        }
    }

    public void ReturnFromPause()
    {
        FadeOff();

        if(!DOTween.IsTweening(pause))
        {
            pause.DOScale(Vector3.zero, .4F)
                .SetEase(Ease.InBack)
                .OnComplete(() => pause.gameObject.SetActive(false));
        }
    }

    public void ReturnFromFail()
    {
        FadeOff();

        failCG.DOFade(0, .5F).OnComplete(() => fail.gameObject.SetActive(false));
    }

    public void PrintSum(int sum = 0)
    {
        sumText.text = sum.ToString();

        float ratio = Mathf.Max(1 - (sum * .05F), 0);

        sumFrame.DOColor(new Color(1, ratio, ratio), .25F).SetTarget(sumFrame);

        sumTextRT.DOScale(1.5F, .125F)
            .OnComplete(() => sumTextRT.DOScale(1, .125F));
    }

    public void Hit21()
    {
        DOTween.Kill(sumFrame);

        sumText.text = "21";

        sumText.DOColor(Color.green, .25F);
        sumFrame.DOColor(Color.green, .25F);

        sumFrameRT.DOScale(1.15F, .125F);
        sumFrameRT.DOScale(1, .125F).SetDelay(.625F)
            .OnComplete(() =>
            {
                sumText.DOColor(Color.white, .25F);
                PrintSum(sm.sum);
            });
    }

    public void UpdateButton(int buttonId, bool enable)
    {
        Button button = null;

        if (buttonId == 0) button = undoButton;
        if (buttonId == 1) button = shuffleButton;
        if (buttonId == 2) button = hintButton;

        button.interactable = enable;
    }

    public void UpdateButtonComponent(int rightId)
    {
        TextMeshProUGUI text = null;
        GameObject circle = null;

        switch(rightId)
        {
            case 0:
                text = undoRightsText;
                circle = undoRightsCircle;
                break;
            case 1:
                text = shuffleRightsText;
                circle = shuffleRightsCircle;
                break;
            case 2:
                text = hintRightsText;
                circle = hintRightsCircle;
                break;
        }

        int rights = DataHandler.GetRights(rightId);

        if (rights != 0) text.text = rights.ToString();
        else text.text = "+";
    }

    public void UpdateUndo()
    {
        int allMovesCount = undo.GetAllMovesCount();
        if (allMovesCount == 0)
        {
            UpdateButton(0, false);
        }
        else
        {
            UpdateButton(0, true);
        }

        UpdateButtonComponent(0);
    }

    public IEnumerator BlinkShuffle(bool close = false)
    {
        if (close) UpdateButton(1, false);
        yield return new WaitForSecondsRealtime(tm.shuffleSpeed + .1F);
        if (DataHandler.GetRights(1) > 0) UpdateButton(1, true);
    }

    public void FakeAd()
    {
        fakeAd.SetActive(true);
    }

    public void ToggleButton(bool active, int buttonId = 3)
    {
        if (buttonId == 0)
        {
            undoButton.gameObject.SetActive(active);
        }
        else if (buttonId == 1)
        {
            shuffleButton.gameObject.SetActive(active);
        }
        else if (buttonId == 2)
        {
            hintButton.gameObject.SetActive(active);
        }
        else
        {
            undoButton.gameObject.SetActive(active);
            shuffleButton.gameObject.SetActive(active);
            hintButton.gameObject.SetActive(active);
        }
    }

    #region Tutorial

    public void StartTutorial(Sprite sprite, Vector2 position, Vector2 size, Vector2 pointerPosition, bool delay = true)
    {
        StartCoroutine(Delay());

        IEnumerator Delay()
        {
            if (delay) yield return Extension.Wait.full;

            PointerOn();
            SpotOn();

            pointer.anchoredPosition = pointerPosition;

            spot.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
            spot.anchoredPosition = position;
            spot.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = size;

            pointerSeq.Play();
        }
    }

    public void EndTutorial()
    {
        pointerSeq.Pause();

        Color color = pointerImage.color;
        color.a = 0;
        pointerImage.DOColor(color, .5F)
            .OnComplete(() => pointer.gameObject.SetActive(false));

        color = spotMask.color;
        color.a = 0;
        spotMask.DOColor(color, .5F)
            .OnComplete(() => spot.gameObject.SetActive(false));
    }

    void PointerOn()
    {
        pointer.gameObject.SetActive(true);

        Color color = pointerImage.color;
        color.a = 0;
        pointerImage.color = color;

        pointerImage.DOColor(Color.white, .5F);
    }

    void SpotOn()
    {
        spot.gameObject.SetActive(true);

        Color color = spotMask.color;
        color.a = 0;
        spotMask.color = color;

        spotMask.DOColor(spotColor, .5F);
    }

    #endregion

    public void DisplayCountdown(int countdown)
    {
        int minutes = countdown / 60;
        string minutesInString = minutes.ToString().Length > 1 ? minutes.ToString() : "0" + minutes;

        int seconds = countdown % 60;
        string secondsInString = seconds.ToString().Length > 1 ? seconds.ToString() : "0" + seconds;

        countdownText.text = minutesInString + ":" + secondsInString;
    }

    public void CountdownOn()
    {
        countdownText.gameObject.SetActive(true);
    }

    public void CountdownOff()
    {
        countdownText.gameObject.SetActive(false);
    }

    public void Goldenize(Tile tile)
    {
        tile.transform.GetChild(4).gameObject.SetActive(true);
    }

    public void GoldenCard(int value, int capacity, bool display = false)
    {
        if (!display)
        {
            goldenBoard.SetActive(false);
        }
        else
        {
            goldenBoard.SetActive(true);
            goldenText.text = value + " / " + capacity;
        }
    }

    void FadeOn()
    {
        Color color = fade.color;
        color.a = 0;
        fade.color = color;
        fade.Fade(.9F);
    }

    void FadeOff(float delay = .25F)
    {
        StartCoroutine(Delay());

        IEnumerator Delay()
        {
            yield return new WaitForSecondsRealtime(delay);

            fade.Fade(0);
        }
    }

    public void BoardOn()
    {
        DOTween.Kill(fade);
        DOTween.Kill(failCG);

        fade.DOFade(.5F, .5F).SetTarget(fade);
        failCG.DOFade(0, .5F).SetTarget(failCG);
    }

    public void BoardOff()
    {
        DOTween.Kill(fade);
        DOTween.Kill(failCG);

        fade.DOFade(.9F, .5F).SetTarget(fade);
        failCG.DOFade(1, .5F).SetTarget(failCG);
    }

    public void StartLevelComponents()
    {
        powerUps.anchoredPosition = new Vector2(0, -2.5F);
        powerUps.DOLocalMoveY(0, .5F).SetDelay(.25F);

        Color color = slotsSprite.color;
        color.a = 0;
        slotsSprite.color = color;
        slotsSprite.DOFade(1, .5F).SetDelay(.25F);

        levelComponents.alpha = 0;
        levelComponents.DOFade(1, .5F).SetDelay(.25F);
    }

    void EndLevelComponents()
    {
        powerUps.DOLocalMoveY(-2.5F, .5F);

        slotsSprite.DOFade(0, .5F);

        levelComponents.DOFade(0, .5F);
    }
}