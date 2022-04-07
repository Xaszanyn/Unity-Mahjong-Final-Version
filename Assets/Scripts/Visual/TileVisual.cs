using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public static class TileVisual
{
    #region Movement

    public static void Move(this Tile tile, Vector3 destination, float time, float delay = 0) // ease
    {
        tile.transform.DOLocalMove(destination, time).SetDelay(delay);
    }

    public static void MoveToSlot(this Tile tile, Vector3 destination) //, EASELERÝ KOY
    {
        tile.transform.position = tile.transform.position + (Vector3.back * 20);

        tile.transform.DOMove(destination, .5F);

        tile.transform.DOScale(.75F / tile.transform.lossyScale.x, .5F);
    }

    public static void MoveFromSlot(this Tile tile, Vector3 destination) // EASE
    {
        tile.transform.DOMove(destination, .5F);
        tile.transform.DOScale(Vector2.one, .5F);
    }

    public static void Disappear(this Tile tile, float time = .5F) // ease
    {
        tile.transform.DOScale(0, time);
    }

    #endregion

    #region Sprite

    public static void Valuate(this Tile tile, Sprite sprite, Sprite lockedSprite)
    {
        tile.background.sprite = sprite;
        tile.lockedBackground.sprite = lockedSprite;
    }

    public static void SpriteLock(this Tile tile)
    {
        tile.lockedBackground.DOFade(1, .5F);
    }

    public static void SpriteUnlock(this Tile tile)
    {
        tile.lockedBackground.DOFade(0, .5F);
    }

    #endregion
}