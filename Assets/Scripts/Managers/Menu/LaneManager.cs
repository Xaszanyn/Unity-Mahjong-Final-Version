using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LaneManager : MonoBehaviour
{
    [SerializeField] RectTransform lane;

    public void GoShop()
    {
        lane.DOAnchorPosX(1620, .5F);
    }

    public void GoMenu()
    {
        lane.DOAnchorPosX(540, .5F);
    }

    public void GoDailyChallenge()
    {
        lane.DOAnchorPosX(-540, .5F);
    }
}
