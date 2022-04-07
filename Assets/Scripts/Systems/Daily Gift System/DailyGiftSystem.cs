using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DailyGiftSystem : MonoBehaviour
{
    #region System

    bool levelLoaded;

    bool giftClaimed;

    [SerializeField] GiftLine giftLine;

    public static int secondsToResetGift = 500; // 34500

    int seconds;

    void Awake()
    {
        CalculateResetting();
        StartCoroutine(Tick());
    }

    void Start()
    {
        InitializeGifts();
    }

    void CalculateResetting()
    {
        int secondsPassed = (int)(System.DateTime.Now - GetLastSeizedTime()).TotalSeconds;

        if (secondsPassed >= secondsToResetGift)
        {
            ResetGift();
            StartCountDown();
        }
        else
        {
            StartCountDown(secondsToResetGift - secondsPassed);
        }
    }

    void StartCountDown(int remainingSeconds = -1)
    {
        if (remainingSeconds == -1) seconds = secondsToResetGift;
        else seconds = remainingSeconds;
    }

    void LogLastSeizedTime()
    {
        DataHandler.LogLastSeizedGiftTime();
    }

    System.DateTime GetLastSeizedTime()
    {
        return DataHandler.GetLastSeizedGiftTime();
    }

    void SetSeizedGiftPhase(int value = -1)
    {
        DataHandler.SetSeizedGiftPhase(value == -1 ? GetSeizedGiftPhase() + 1 : value);
    }

    int GetSeizedGiftPhase()
    {
        return DataHandler.GetSeizedGiftPhase();
    }

    void ResetGift()
    {
        LogLastSeizedTime();

        seconds = secondsToResetGift;

        SetSeizedGiftPhase(0);
    }

    void DecreaseSeconds()
    {
        seconds = Mathf.Max(seconds - 1, 0);
    }

    IEnumerator Tick()
    {
        while (true)
        {
            Display();

            yield return Extension.Wait.full;

            DecreaseSeconds();

            if (seconds == 0) ResetGift();
        }
    }

    public void OpenDailyGift()
    {
        OpenPanel();

        if (!levelLoaded)
        {
            Load();
            levelLoaded = true;
        }

        Focus();
    }

    public void CloseDailyGift()
    {
        ClosePanel();
        levelLoaded = false;
    }

    #endregion

    #region UI

    [SerializeField] RectTransform panel;

    [SerializeField] RectTransform giftSlide;

    [SerializeField] GameObject giftTemplate;

    [SerializeField] RectTransform failText;

    void Display()
    {
        //Debug.Log("CD: " + seconds);
    }

    void OpenPanel()
    {
        panel.gameObject.SetActive(true);
        panel.anchoredPosition = new Vector2(panel.anchoredPosition.x, -1920);
        panel.DOAnchorPosY(0, .5F);
    }

    void ClosePanel()
    {
        panel.DOAnchorPosY(-1920, .5F)
            .OnComplete(() => panel.gameObject.SetActive(true));
    }

    

    void InitializeGifts()
    {
        giftIcons = new List<Sprite>();

        for (int i = 0; i < giftLine.giftLine.Count; i++)
        {
            GiftLine.Gift selectedGift = giftLine.giftLine[i];

            var gift = Instantiate(giftTemplate, giftSlide);

            Transform content = gift.transform.GetChild(0);

            Sprite sprite = giftLine.icons[(int)selectedGift.gift];

            content.GetChild(0).GetComponent<Image>().sprite = sprite;

            content.GetChild(1).GetComponent<TextMeshProUGUI>().text = "x" + selectedGift.amount;

            giftIcons.Add(sprite);
        }
    }

    void Focus()
    {
        int phase = GetSeizedGiftPhase();

        int focus = 2340 - (190 * phase);

        giftSlide.DOLocalMoveX(focus, .5F);

        for (int i = 0; i < giftSlide.childCount; i++)
        {
            Transform selectedGift = giftSlide.GetChild(i);

            RectTransform outer = selectedGift.GetComponent<RectTransform>();
            RectTransform inner = selectedGift.GetChild(0).GetComponent<RectTransform>();

            if (i == phase)
            {
                outer.DOSizeDelta(new Vector2(220, 250), .5F);

                inner.DOScale(Vector2.one * 1.5F, .5F);
            }
            else
            {
                outer.DOSizeDelta(new Vector2(140, 160), .5F);

                inner.DOScale(Vector2.one, .5F);
            }
        }
    }

    void UISuccess()
    {
        slotsRT.DOLocalMoveY(-1150, .25F).SetDelay(.5F)
            .OnComplete(() => {
                giftIcon.DOScale(Vector2.one, .25F);
                giftButtonRT.DOScale(Vector2.one, .25F).SetDelay(.25F);
            });

        giftIconImage.sprite = giftIcons[GetSeizedGiftPhase()];
    }

    void UIAgain()
    {
        giftIcon.DOScale(Vector2.zero, .25F);
        giftButtonRT.DOScale(Vector2.zero, .25F)
            .OnComplete(() => {
                slotsRT.DOLocalMoveY(-550, .25F);
                giftClaimed = false;
            });
    }

    public void UIFail()
    {
        failText.DOLocalMoveY(730, .25F)
            .OnComplete(() => failText.DOLocalMoveY(500, .25F).SetDelay(1));
    }

    #endregion

    #region Game

    [SerializeField] GameObject levelTemplate;

    int sum;

    List<RectTransform> tiles = new List<RectTransform>();

    [SerializeField] List<RectTransform> deck;

    [SerializeField] RectTransform[] slots;

    [SerializeField] RectTransform slotsRT;

    [SerializeField] Image giftIconImage;
    [SerializeField] RectTransform giftIcon;
    [SerializeField] RectTransform giftButtonRT;

    List<Sprite> giftIcons;

    int count;

    [SerializeField] float moveSpeed;

    [SerializeField] Color[] palette;

    [SerializeField] List<IntegerList> trioList;
    List<int> designatedValues;

    GameObject currentLevel;

    void Load(bool random = true)
    {
        if (currentLevel != null)
        {
            DestroyLevel();
        }

        if (random)
        {
            designatedValues = Designate(levelTemplate.transform.childCount);

            currentLevel = Instantiate(levelTemplate, panel.transform);

            currentLevel.GetComponent<RectTransform>().DOScale(Vector2.one, .5F);

            for (int i = 0; i < currentLevel.transform.childCount; i++)
            {
                Transform tile = currentLevel.transform.GetChild(i);

                deck.Add(tile.GetComponent<RectTransform>());

                EventTrigger trigger = tile.GetComponent<EventTrigger>();

                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerDown;
                entry.callback.AddListener((eventData) => Move(tile.GetComponent<RectTransform>()));

                trigger.triggers.Add(entry);
            }
        }

        List<int> Designate(int amount)
        {
            List<int> temporaryList = new List<int>();
            List<int> designatedList = new List<int>();

            for(int i = 0; i < amount / 3; i++)
            {
                IntegerList list = trioList[UnityEngine.Random.Range(0, trioList.Count)];

                designatedList.AddRange(list.list);
            }

            int chance = UnityEngine.Random.Range(1, 101);

            if (chance <= 20)
            {
                Debug.Log("SUCCESS");

                return designatedList;
            }
            else if (chance <= 50)
            {
                Debug.Log("MAYBE YOU CAN DO IT");

                for(int i = 3; i < designatedList.Count; i++)
                {
                    temporaryList.Add(designatedList[3]);

                    designatedList.RemoveAt(3);
                }

                temporaryList.Shuffle();

                designatedList.AddRange(temporaryList);

                return designatedList;
            }
            else
            {
                Debug.Log("HOPE GOD IS ON YOUR SIDE BRUV");

                designatedList.Shuffle();

                return designatedList;
            }
        }
    }
    void Fail()
    {
        ResetLevel();
        DestroyLevel();

        UIFail();

        StartCoroutine(Delay());

        IEnumerator Delay()
        {
            yield return Extension.Wait.full;
            Load();
        }
    }

    void DestroyLevel()
    {
        GameObject current = currentLevel;

        current.GetComponent<RectTransform>().DOScale(Vector2.zero, .5F)
            .OnComplete(() => Destroy(current));
    }

    void ResetLevel()
    {
        sum = 0;
        deck.Clear();
        tiles.Clear();
        count = 0;
    }

    bool IsSlotsFull()
    {
        return count == slots.Length;
    }

    public void Move(RectTransform tile)
    {
        bool isSlotsFull = IsSlotsFull();

        DailyGiftTile tileProperties = tile.GetComponent<DailyGiftTile>();

        if (tileProperties.clicked || isSlotsFull) return;

        int designatedValue = designatedValues[0];
        designatedValues.RemoveAt(0);

        tileProperties.InvokeTiles();

        tileProperties.clicked = true;

        deck.Remove(tile);

        sum += designatedValue;

        tiles.Add(tile);

        tile.DOMove(slots[count].position, moveSpeed);

        tile.transform.SetAsLastSibling();

        Reveal(tile.GetChild(0).GetComponent<TextMeshProUGUI>(), designatedValue);

        count++;

        bool isComplete = Check();

        if (isSlotsFull && !isComplete || sum > 21) Fail();
    }

    void Reveal(TextMeshProUGUI text, int value)
    {
        text.text = value.ToString();

        text.DOColor(palette[value - 1], moveSpeed);
    }

    bool Check()
    {
        if (sum == 21)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                RectTransform tile = tiles[i];

                tile.DOScale(Vector2.zero, .25F).SetDelay(i * .1F + moveSpeed)
                    .OnComplete(() => Destroy(tile.gameObject, 1));
            }

            tiles.Clear();

            count = sum = 0;

            if (deck.Count == 0)
            {
                Success();
            }

            return true;
        }
        else return false;
    }

    void Success()
    {
        ResetLevel();
        UISuccess();
    }

    public void Claim()
    {
        if (giftClaimed) return;

        giftClaimed = true;

        GetGift();

        UIAgain();

        SetSeizedGiftPhase();
        Focus();

        StartCoroutine(Delay());

        IEnumerator Delay()
        {
            yield return Extension.Wait.half;
            Load();
        }
    }

    void GetGift()
    {
        GiftLine.Gift giftToSeize = giftLine.giftLine[GetSeizedGiftPhase()];

        Debug.Log(giftToSeize.gift);
        Debug.Log(giftToSeize.amount);
        

        // GET THE PRIZE
    }

    #endregion

    void Update() ///
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetSeizedGiftPhase(0);
            Focus();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetSeizedGiftPhase(1);
            Focus();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetSeizedGiftPhase(2);
            Focus();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetSeizedGiftPhase(3);
            Focus();
        }
    }

    [System.Serializable] public struct IntegerList
    {
        public List<int> list;
    }
}