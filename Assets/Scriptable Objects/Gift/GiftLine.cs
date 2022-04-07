using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gift Line", menuName = "Scriptable Objects/Gift Line")]
public class GiftLine : ScriptableObject
{
    public List<Gift> giftLine;

    public Sprite[] icons;

    [System.Serializable] public struct Gift
    {
        public GiftType gift;
        public int amount;

        public enum GiftType
        {
            undo, shuffle, blow21, coin, treasure
        }
    }
}