using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> CurrentMatches;

    //public delegate void OnMatchesFound();
    //public static event OnMatchesFound Found;

    private void Start()
    {
        board = FindObjectOfType<Board>();
        CurrentMatches = new List<GameObject>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private List<GameObject> IsAdjacentBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        var currentDots = new List<GameObject>();
        UnionAdjacentLists(dot1);
        UnionAdjacentLists(dot2);
        UnionAdjacentLists(dot3);
        return currentDots;
    }

    private List<GameObject> IsRowBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        var currentDots = new List<GameObject>();
        UnionRowLists(dot1);
        UnionRowLists(dot2);
        UnionRowLists(dot3);
        return currentDots;
    }

    private List<GameObject> IsColumnBomb (Dot dot1, Dot dot2, Dot dot3)
    {
        var currentDots = new List<GameObject>();
        UnionColumnLists(dot1);
        UnionColumnLists(dot2);
        UnionColumnLists(dot3);
        return currentDots;
    }

    private void UnionRowLists(Dot dot)
    {
        if (dot.IsRowBomb)
            CurrentMatches.Union(GetRowPieces(dot.Row));
    }

    private void UnionColumnLists(Dot dot)
    {
        if (dot.IsColumnBomb)
            CurrentMatches.Union(GetColumnPieces(dot.Column));
    }

    private void UnionAdjacentLists(Dot dot)
    {
        if (dot.IsAdjacentBomb)
            CurrentMatches.Union(GetAdjacentPieces(dot.Column, dot.Row));
    }    

    private void GetNearbyPieces(GameObject dot1, GameObject dot2, GameObject dot3)
    {
        AddToListAndMatch(dot1);
        AddToListAndMatch(dot2);
        AddToListAndMatch(dot3);
    }

    private void AddToListAndMatch(GameObject dot)
    {
        if (!CurrentMatches.Contains(dot))
        {
            CurrentMatches.Add(dot);
            var color = CurrentMatches[0].tag.Split(' ')[0];
            switch (color)
            {
                case "Blue":
                    Bullet.bulletClip.Enqueue(Color.blue);
                    break;
                case "Green":
                    Bullet.bulletClip.Enqueue(Color.green);
                    break;
                case "Orange":
                    Bullet.bulletClip.Enqueue(new Color(1.0f, 0.64f, 0.0f));
                    break;
                case "Purple":
                    Bullet.bulletClip.Enqueue(Color.magenta);
                    break;
                case "Red":
                    Bullet.bulletClip.Enqueue(Color.red);
                    break;
                case "Light":
                    Bullet.bulletClip.Enqueue(Color.yellow);
                    break;
                default:
                    break;
            }
        }
        dot.GetComponent<Dot>().IsMatched = true;
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);
        for (int i = 0; i < board.Width; i++)
            for (int j = 0; j < board.Height; j++)
            {
                GameObject currentDot = board.AllDots[i, j];
                if (currentDot != null)
                {
                    if (i > 0 && i < board.Width - 1)
                    {
                        GameObject leftDot = board.AllDots[i - 1, j];  
                        GameObject rightDot = board.AllDots[i + 1, j];  
                        if (leftDot != null && rightDot != null)
                            if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                            {
                                CurrentMatches.Union(IsRowBomb(leftDot.GetComponent<Dot>(), currentDot.GetComponent<Dot>(), rightDot.GetComponent<Dot>()));

                                CurrentMatches.Union(IsColumnBomb(leftDot.GetComponent<Dot>(), currentDot.GetComponent<Dot>(), rightDot.GetComponent<Dot>()));

                                CurrentMatches.Union(IsAdjacentBomb(leftDot.GetComponent<Dot>(), currentDot.GetComponent<Dot>(), rightDot.GetComponent<Dot>()));

                                GetNearbyPieces(leftDot, currentDot, rightDot);
                            }
                    }
                    if (j > 0 && j < board.Height - 1)
                    {
                        GameObject upDot = board.AllDots[i, j + 1];
                        GameObject downDot = board.AllDots[i, j - 1];
                        if (upDot != null && downDot != null)
                            if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag)
                            {
                                CurrentMatches.Union(IsColumnBomb(upDot.GetComponent<Dot>(), currentDot.GetComponent<Dot>(), downDot.GetComponent<Dot>()));

                                CurrentMatches.Union(IsRowBomb(upDot.GetComponent<Dot>(), currentDot.GetComponent<Dot>(), downDot.GetComponent<Dot>()));

                                CurrentMatches.Union(IsAdjacentBomb(upDot.GetComponent<Dot>(), currentDot.GetComponent<Dot>(), downDot.GetComponent<Dot>()));

                                GetNearbyPieces(upDot, currentDot, downDot);
                            }
                    }
                }
            }
    }

    public void MatchSameColorPieces(string color)
    {
        for (int i = 0; i < board.Width; i++)
            for (int j = 0; j < board.Height; j++)
            {
                if (board.AllDots[i, j] != null)
                    if (board.AllDots[i, j].tag == color)
                        board.AllDots[i, j].GetComponent<Dot>().IsMatched = true;
            }
    }
    private List<GameObject> GetColumnPieces(int column)
    {
        var dots = new List<GameObject>();
        for (int i = 0; i < board.Height; i++)
            if (board.AllDots[column, i] != null)
            {
                Dot dot = board.AllDots[column, i].GetComponent<Dot>();
                if (dot.IsRowBomb)
                    dots.Union(GetRowPieces(i)).ToList();
                dots.Add(board.AllDots[column, i]);
                dot.IsMatched = true;
            }
        return dots;
    }

    private List <GameObject> GetAdjacentPieces(int column, int row)
    {
        var dots = new List<GameObject>();
        for (int i = column - 1; i <= column + 1; i++)
            for (int j = row - 1; j <= row + 1; j++)
            {
                if (i >= 0 && i < board.Width && j >= 0 && j < board.Height 
                    && board.AllDots[i, j] != null)
                {
                    dots.Add(board.AllDots[i, j]);
                    board.AllDots[i, j].GetComponent<Dot>().IsMatched = true;
                }
            }
        return dots;
    }
    
    private List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.Width; i++)
            if (board.AllDots[i, row] != null)
            {
                Dot dot = board.AllDots[i, row].GetComponent<Dot>();
                if (dot.IsColumnBomb)
                    dots.Union(GetColumnPieces(i)).ToList();
                dots.Add(board.AllDots[i, row]);
                dot.IsMatched = true;
            }
        return dots;
    }

    public void CheckBombs()
    {     
        //Did the player move something?
        if (board.CurrentDot != null)
        {
            //Is the piece they moved matched?
            if (board.CurrentDot.IsMatched)
            {
                board.CurrentDot.IsMatched = false;
                MakeBombType(board.CurrentDot,board.CurrentDot);     
            }
            //Is the other piece they moved matched?
            else if (board.CurrentDot.OtherDot != null)
            {
                Dot otherDot = board.CurrentDot.OtherDot.GetComponent<Dot>();
                if (otherDot.IsMatched)
                {
                    otherDot.IsMatched = false;
                    MakeBombType(board.CurrentDot, otherDot);
                }
            }
        }
    }

    public void CheckColorBombs()
    {
        if (board.CurrentDot != null)
        {
            if (board.CurrentDot.IsMatched)
            {
                board.CurrentDot.IsMatched = false;
                board.CurrentDot.MakeColorBomb();
            }
        }
    }

    public void CheckAdjacentBombs()
    {
        if (board.CurrentDot != null)
        {
            if (board.CurrentDot.IsMatched)
            {
                board.CurrentDot.IsMatched = false;
                board.CurrentDot.MakeAdjacentBomb();
            }
        }
    }

    private void MakeBombType(Dot dot, Dot otherDot)
    {
        if ((dot.SwipeAngle > -45 && dot.SwipeAngle <= 45)
         || dot.SwipeAngle < -135 || dot.SwipeAngle >= 135)
            otherDot.MakeRowBomb();
        else
            otherDot.MakeColumnBomb();
    }
}