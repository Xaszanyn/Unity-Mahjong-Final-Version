using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FakeAd : MonoBehaviour
{
    [SerializeField] Sprite[] ads;
    Image image;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    void OnEnable()
    {
        image.sprite = ads[Random.Range(0, ads.Length)];
        StartCoroutine(Close());
    }

    IEnumerator Close()
    {
        yield return new WaitForSecondsRealtime(1.5F);
        transform.parent.gameObject.SetActive(false);
    }
}
