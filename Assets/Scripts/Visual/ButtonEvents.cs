using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ButtonEvents : MonoBehaviour
{
    public void Grow(RectTransform button)
    {
        button.DOScale(Vector2.one * 1.2F, .25F);
    }

    public void Shrink(RectTransform button)
    {
        button.DOScale(Vector2.one, .25F);
    }
}
