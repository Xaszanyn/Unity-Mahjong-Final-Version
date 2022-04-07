using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using TMPro;

public class Tile : MonoBehaviour
{
    UndoManager undo;

    public int value { get; set; }

    public bool isLocked { get; private set; }

    public UnityAction<Tile> TileClick;
    public List<Tile> blocks;

    [SerializeField] private Transform[] corners;

    public SpriteRenderer background;
    public SpriteRenderer lockedBackground;

    public bool golden;
    public int goldenity;

    public int givenValue { get; private set; }
    [SerializeField] TextMeshPro GIVEN_VALUE;

    void Awake()
    {
        SetGivenValue();

        isLocked = true;
    }

    void OnMouseDown()
    {
        if (!isLocked && !GameManager.gm.isPaused) TileClick?.Invoke(this);
    }

    #region Blocking

    private void BlockClicked(Tile tile)
    {
        blocks.Remove(tile);

        if (blocks.Count == 0) Unlock();

        GameManager.gm.GetUndo().AddBlockedTile(this);
    }

    public void RearrangeBlocks()
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            blocks[i].TileClick -= BlockClicked;
        }

        blocks.Clear();

        Hit();

        if (blocks.Count == 0)
        {
            Unlock();
        }
        else
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                blocks[i].TileClick += BlockClicked;
            }
        }

        void Hit()
        {
            for (int i = 0; i < 4; i++)
            {
                RaycastHit2D[] hits;

                hits = Physics2D.RaycastAll(corners[i].transform.position, Vector3.forward);

                for (int j = 0; j < hits.Length; j++)
                {
                    RaycastHit2D hit = hits[j];

                    if (hit.collider.transform.position.z < transform.position.z - 0.1f && hit.collider.transform.position.z > transform.position.z - 1.2F)
                    {
                        Tile tile = hit.collider.GetComponent<Tile>();

                        if (tile != null && !blocks.Contains(tile)) blocks.Add(tile);
                    }
                }
            }
        }
    }

    #endregion

    #region Lock

    public void Lock()
    {
        isLocked = true;

        this.SpriteLock();
    }

    public void Unlock()
    {
        isLocked = false;

        this.SpriteUnlock();
    }

    public void LockWithOutFade() { isLocked = true; }

    public void UnlockWithOutFade() { isLocked = false; }

    #endregion

    #region Value

    void SetGivenValue()
    {
        string text = GIVEN_VALUE.text;

        if (text == "?")
        {
            givenValue = -1;
        }
        else
        {
            givenValue = int.Parse(text);
        }

        GIVEN_VALUE.gameObject.SetActive(false);
    }

    #endregion

    #region Animation

    public void Animate(int layer, AnimationType animationType, float time)
    {
        int z = (int)Mathf.Round(transform.position.z);

        Vector3 destination = transform.localPosition;

        TileAnimation animation = null;

        switch (animationType)
        {
            case AnimationType.LayerByLayer:
                time += .3F;
                animation = LayerByLayer(destination, z, time, layer, true);
                break;
            case AnimationType.Solitaire:
                time += .4F;
                animation = Solitaire(destination, time, 0);
                break;
            case AnimationType.Slide:
                animation = Slide(destination, time, 3);
                break;
            case AnimationType.Assemble:
                animation = Assemble(destination, time);
                break;
            case AnimationType.Spread:
                animation = Spread(destination, time, 3);
                break;
        }

        this.Move(animation.destination, animation.time, animation.delay);
    }

    TileAnimation LayerByLayer(Vector3 destination, int z, float maximumTime, int layer, bool direction)
    {
        float time = OrderByLayer();

        bool even = (z) % 2 == 0;

        if (direction) transform.position += Vector3.left * (even ? 8F : -8F);
        else transform.position += Vector3.up * (even ? 15F : -15F);

        StartCoroutine(Count(.3F + time));

        return new TileAnimation(destination, .3F, time);

        float OrderByLayer()
        {
            if (layer == 1) return 0;

            return maximumTime - ((layer - 1 + z) / (float)(layer - 1)) * maximumTime;
        }
    }

    TileAnimation Solitaire(Vector3 destination, float maximumTime, int direction)
    {
        switch (direction)
        {
            case 0:
                transform.position = Vector3.up * 15F;
                break;
            case 1:
                transform.position = Vector3.right * 8F;
                break;
            case 2:
                transform.position = Vector3.down * 15F;
                break;
            case 3:
                transform.position = Vector3.left * 8F;
                break;
        }

        float random = Random.Range(0, maximumTime);

        StartCoroutine(Count(.4F + random));

        return new TileAnimation(destination, .4F, random);
    }

    TileAnimation Slide(Vector3 destination, float maximumTime, int direction)
    {
        switch (direction)
        {
            case 0:
                transform.position += Vector3.up * 20;
                break;
            case 1:
                transform.position += Vector3.right * 20;
                break;
            case 2:
                transform.position += Vector3.down * 20;
                break;
            case 3:
                transform.position += Vector3.left * 20;
                break;
        }

        StartCoroutine(Count(maximumTime));

        return new TileAnimation(destination, maximumTime);
    }

    TileAnimation Assemble(Vector3 destination, float maximumTime)
    {
        int randomangle = Random.Range(0, 360);

        transform.position = new Vector3(0, 0, 0);

        transform.position += new Vector3(Mathf.Cos(randomangle) * 10, Mathf.Sin(randomangle) * 10, 0);

        StartCoroutine(Count(maximumTime));

        return new TileAnimation(destination, maximumTime);
    }

    TileAnimation Spread(Vector3 destination, float maximumTime, int direction)
    {
        Vector3 from = transform.position;

        switch (direction)
        {
            case 0: from.y = 10; break;
            case 1: from.x = 6.5F; break;
            case 2: from.y = -10; break;
            case 3: from.x = -6.5F; break;
        }

        transform.position = from;

        float time = Mathf.Min(Mathf.Abs(Vector3.Distance(destination, from)) / 10F, maximumTime);

        StartCoroutine(Count(time));

        return new TileAnimation(destination, time);
    }

    IEnumerator Count(float time)
    {
        yield return new WaitForSecondsRealtime(time);

        TileManager.instance.InitialFinisher();
    }

    class TileAnimation
    {
        public Vector3 destination;
        public float time;
        public float delay;

        public TileAnimation(Vector3 destination, float time, float delay = 0)
        {
            this.destination = destination;
            this.time = time;
            this.delay = delay;
        }
    }

    #endregion
}