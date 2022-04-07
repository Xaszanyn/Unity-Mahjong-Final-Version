using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UndoManager : MonoBehaviour
{

    [SerializeField] TileManager tm;
    List<Move> allMoves = new List<Move>();

    [System.Serializable] public class Move
    {
        public Tile tile;
        public Vector3 tilesLocation;
        public List<Tile> blockedTiles;
        public int sum;

        public Move()
        {
            this.tile = null;
            this.tilesLocation = Vector3.zero;
            this.blockedTiles = new List<Tile>();
            this.sum = -1;
        }
    }

    public void AddMove(Tile tile, int sum)
    {
        Move move = new Move();
        move.tile = tile;
        move.tilesLocation = tile.transform.position;
        move.sum = sum;

        allMoves.Add(move);
    }

    public void Undo()
    {
        if (allMoves.Count == 0) return;

        Move lastMove = allMoves[allMoves.Count - 1];
        allMoves.Remove(lastMove);

        lastMove.tile.MoveFromSlot(lastMove.tilesLocation);

        lastMove.tile.UnlockWithOutFade();

        foreach (Tile other in lastMove.blockedTiles)
        {
            other.Lock();
            other.blocks.Add(lastMove.tile);
        }

        tm.UndoTile(lastMove.tile, lastMove.sum);
    }

    public void AddBlockedTile(Tile tile)
    {
        if (allMoves.Count == 0) return;
        
        allMoves[allMoves.Count - 1].blockedTiles.Add(tile);
    }

    public void ResetMoves()
    {
        allMoves.Clear();
    }

    public int GetAllMovesCount()
    {
        return allMoves.Count;
    }
}
