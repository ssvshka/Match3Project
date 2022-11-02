using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();

    private void Start()
    {
        board = FindObjectOfType<Board>();    
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
        if (dot.isRowBomb)
            currentMatches.Union(GetRowPieces(dot.row));
    }

    private void UnionColumnLists(Dot dot)
    {
        if (dot.isColumnBomb)
            currentMatches.Union(GetColumnPieces(dot.column));
    }

    private void UnionAdjacentLists(Dot dot)
    {
        if (dot.isAdjacentBomb)
            currentMatches.Union(GetAdjacentPieces(dot.column, dot.row));
    }    

    private void GetNearbyPieces(GameObject dot1, GameObject dot2, GameObject dot3)
    {
        AddToListAndMatch(dot1);
        AddToListAndMatch(dot2);
        AddToListAndMatch(dot3);
    }

    private void AddToListAndMatch(GameObject dot)
    {
        if (!currentMatches.Contains(dot))
            currentMatches.Add(dot);
        dot.GetComponent<Dot>().isMatched = true;
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);
        for (int i = 0; i < board.width; i++)
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentDot = board.allDots[i, j];
                //Dot currentDotDot = currentDot.GetComponent<Dot>();
                if (currentDot != null)
                {
                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftDot = board.allDots[i - 1, j];
                        //Dot leftDotDot = leftDot.GetComponent<Dot>();  
                        GameObject rightDot = board.allDots[i + 1, j];
                        //Dot rightDotDot = rightDot.GetComponent<Dot>();  
                        if (leftDot != null && rightDot != null)
                            if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                            {
                                currentMatches.Union(IsRowBomb(leftDot.GetComponent<Dot>(), currentDot.GetComponent<Dot>(), rightDot.GetComponent<Dot>()));

                                currentMatches.Union(IsColumnBomb(leftDot.GetComponent<Dot>(), currentDot.GetComponent<Dot>(), rightDot.GetComponent<Dot>()));

                                currentMatches.Union(IsAdjacentBomb(leftDot.GetComponent<Dot>(), currentDot.GetComponent<Dot>(), rightDot.GetComponent<Dot>()));

                                GetNearbyPieces(leftDot, currentDot, rightDot);
                            }
                    }
                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upDot = board.allDots[i, j + 1];
                        //Dot upDotDot = upDot.GetComponent<Dot>();
                        GameObject downDot = board.allDots[i, j - 1];
                        //Dot downDotDot = downDot.GetComponent<Dot>(); 
                        if (upDot != null && downDot != null)
                            if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag)
                            {
                                currentMatches.Union(IsColumnBomb(upDot.GetComponent<Dot>(), currentDot.GetComponent<Dot>(), downDot.GetComponent<Dot>()));

                                currentMatches.Union(IsRowBomb(upDot.GetComponent<Dot>(), currentDot.GetComponent<Dot>(), downDot.GetComponent<Dot>()));

                                currentMatches.Union(IsAdjacentBomb(upDot.GetComponent<Dot>(), currentDot.GetComponent<Dot>(), downDot.GetComponent<Dot>()));

                                GetNearbyPieces(upDot, currentDot, downDot);
                            }
                    }
                }
            }
    }

    public void MatchSameColorPieces(string color)
    {
        for (int i = 0; i < board.width; i++)
            for (int j = 0; j < board.height; j++)
            {
                if (board.allDots[i, j] != null)
                    if (board.allDots[i, j].tag == color)
                        board.allDots[i, j].GetComponent<Dot>().isMatched = true;
            }
    }
    private List<GameObject> GetColumnPieces(int column)
    {
        var dots = new List<GameObject>();
        for (int i = 0; i < board.height; i++)
            if (board.allDots[column, i] != null)
            {
                Dot dot = board.allDots[column, i].GetComponent<Dot>();
                if (dot.isRowBomb)
                    dots.Union(GetRowPieces(i)).ToList();
                dots.Add(board.allDots[column, i]);
                dot.isMatched = true;
            }
        return dots;
    }

    private List <GameObject> GetAdjacentPieces(int column, int row)
    {
        var dots = new List<GameObject>();
        for (int i = column - 1; i <= column + 1; i++)
            for (int j = row - 1; j <= row + 1; j++)
            {
                if (i >= 0 && i < board.width && j >= 0 && j < board.height 
                    && board.allDots[i, j] != null)
                {
                    dots.Add(board.allDots[i, j]);
                    board.allDots[i, j].GetComponent<Dot>().isMatched = true;
                }
            }
        return dots;
    }
    
        private List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
            if (board.allDots[i, row] != null)
            {
                Dot dot = board.allDots[i, row].GetComponent<Dot>();
                if (dot.isColumnBomb)
                    dots.Union(GetColumnPieces(i)).ToList();
                dots.Add(board.allDots[i, row]);
                dot.isMatched = true;
            }
        return dots;
    }

    public void CheckBombs()
    {     
        //Did the player move something?
        if (board.currentDot != null)
        {
            //Is the piece they moved matched?
            if (board.currentDot.isMatched)
            {
                board.currentDot.isMatched = false;
                MakeBombType(board.currentDot,board.currentDot);     
            }
            //Is the other piece they moved matched?
            else if (board.currentDot.otherDot != null)
            {
                Dot otherDot = board.currentDot.otherDot.GetComponent<Dot>();
                if (otherDot.isMatched)
                {
                    otherDot.isMatched = false;
                    MakeBombType(board.currentDot, otherDot);
                }
            }
        }
    }

    public void CheckColorBombs()
    {
        if (board.currentDot != null)
        {
            if (board.currentDot.isMatched)
            {
                board.currentDot.isMatched = false;
                board.currentDot.MakeColorBomb();
            }
        }
    }

    public void CheckAdjacentBombs()
    {
        if (board.currentDot != null)
        {
            if (board.currentDot.isMatched)
            {
                board.currentDot.isMatched = false;
                board.currentDot.MakeAdjacentBomb();
            }
        }
    }

    private void MakeBombType(Dot dot, Dot otherDot)
    {
        if ((dot.swipeAngle > -45 && dot.swipeAngle <= 45)
         || dot.swipeAngle < -135 || dot.swipeAngle >= 135)
            otherDot.MakeRowBomb();
        else
            otherDot.MakeColumnBomb();
    }
}