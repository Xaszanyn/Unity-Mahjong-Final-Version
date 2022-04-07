using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public static class Extension
{
    #region UI
        
    public static void Fade(this Image image, float value, float duration = .5F, TweenCallback callback = null)
    {
        image.gameObject.SetActive(true);
        image.DOFade(value, duration)
            .OnComplete(callback)
            .OnComplete(() => { if (value == 0) image.gameObject.SetActive(false); });
    }

    public static void Hide(this TextMeshProUGUI text)
    {
        text.alpha = 0;
        text.gameObject.SetActive(false);
    }
    public static void Hide(this Image image)
    {
        Color color = image.color;
        color.a = 0;
        image.color = color;
        image.gameObject.SetActive(false);
    }

    public static void Scale(this RectTransform transform, float value, float duration, TweenCallback callback = null)
    {
        transform.DOScale(new Vector2(value, value), duration)
                .OnComplete(callback);
    }

    #endregion

    public static void Centralize(this Transform transform, float alignAmount)
    {
        float farLeft, farRight, farTop, farBottom;
        farLeft = farBottom = float.MaxValue;
        farRight = farTop = float.MinValue;

        Tile[] renderList = transform.GetComponentsInChildren<Tile>();

        for(int i = 0; i < renderList.Length; i++)
        {
            SpriteRenderer sprite = renderList[i].GetComponentInChildren<SpriteRenderer>();

            if (farLeft > sprite.bounds.min.x) farLeft = sprite.bounds.min.x;
            if (farBottom > sprite.bounds.min.y) farBottom = sprite.bounds.min.y;
            if (farRight < sprite.bounds.max.x) farRight = sprite.bounds.max.x;
            if (farTop < sprite.bounds.max.y) farTop = sprite.bounds.max.y;
        }

        Vector3 centre = new Vector3((farLeft + farRight) / 2, (farTop + farBottom) / 2);
        Vector3 spriteDifference = Vector3.left * .05F;
        Vector3 alignDifference = Vector3.up * (alignAmount + 1.4F);

        transform.position -= centre + spriteDifference - alignDifference;
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random random = new System.Random();

        int count = list.Count;
        while (count > 1)
        {
            count--;

            int randomCount = random.Next(count + 1);
            T value = list[randomCount];
            list[randomCount] = list[count];
            list[count] = value;
        }
    }

    public static class Wait
    {
        public static WaitForSecondsRealtime thrice = new WaitForSecondsRealtime(2);
        public static WaitForSecondsRealtime twice = new WaitForSecondsRealtime(2);
        public static WaitForSecondsRealtime full = new WaitForSecondsRealtime(1);
        public static WaitForSecondsRealtime half = new WaitForSecondsRealtime(.5F);
        public static WaitForSecondsRealtime quarter = new WaitForSecondsRealtime(.25F);
        public static WaitForSecondsRealtime tiny = new WaitForSecondsRealtime(.1F);
        public static WaitForEndOfFrame end = new WaitForEndOfFrame();
    }

    [System.Serializable] public struct ButtonWithContent
    {
        public int buttonId;
        public RectTransform content;
    }
}
