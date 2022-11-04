using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum GameState
{
    wait, move
}

public enum TileKind
{
    Breakable,
    Blank,
    Normal
}

[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileKind tileKind;
}

public class Board : MonoBehaviour
{
    public GameState currentState = GameState.move;
    public int width;
    public int height;
    public GameObject[,] allDots;
    public Dot currentDot;
    public int[] scoreGoals;

    [SerializeField] private int offSet;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject breakableTilePrefab;
    [SerializeField] private GameObject[] dotsColor;
    [SerializeField] private GameObject destroyEffect;
    [SerializeField] private TileType[] boardLayout;
    [SerializeField] private float refillDelay = .5f;
    [SerializeField] private int basePieceValue = 20;
    
    private bool[,] blankSpaces;
    private Jelly[,] breakableTiles;
    
    private int streakValue = 1;
    
    private FindMatches findMatches;
    private ScoreManager scoreManager;
    private SoundManager soundManager;
    private GoalManager goalManager;

    private void Start()
    {
        goalManager = FindObjectOfType<GoalManager>();
        soundManager = FindObjectOfType<SoundManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        findMatches = FindObjectOfType<FindMatches>();
        blankSpaces = new bool[width, height];
        allDots = new GameObject[width, height];
        breakableTiles = new Jelly[width, height];
        SetUp();
    }

    private void GenerateBlankSpaces()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Blank)
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
        }
    }

    private void GenerateBreakableTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
            if (boardLayout[i].tileKind == TileKind.Breakable)
            {
                var tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(breakableTilePrefab, tempPosition, Quaternion.identity);
                breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<Jelly>();
            }
    }

    private void SetUp()
    {
        GenerateBlankSpaces();
        GenerateBreakableTiles();
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    Vector2 tilePosition = new Vector2(i, j);
                    GameObject backgroundTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;
                    backgroundTile.transform.parent = this.transform;
                    backgroundTile.name = $"({i}, {j})";
                    int dotToUse = Random.Range(0, dotsColor.Length);

                    int maxIterations = 0;
                    while (MatchesAt(i, j, dotsColor[dotToUse]) && maxIterations < 100)
                    {
                        dotToUse = Random.Range(0, dotsColor.Length);
                        maxIterations++;
                    }
                    maxIterations = 0;

                    GameObject dot = Instantiate(dotsColor[dotToUse], tempPosition, Quaternion.identity);
                    dot.GetComponent<Dot>().row = j;
                    dot.GetComponent<Dot>().column = i;
                    dot.transform.parent = this.transform;
                    dot.name = $"({i}, {j})";
                    allDots[i, j] = dot;
                }
            }
    }

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1 && row > 1)
        {
            if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                    return true;

            if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
                if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                    return true;
        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
                if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
                    if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                        return true;
            if (column > 1)
                if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
                    if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                        return true;
        }
        return false;
    }

    private bool ColumnOrRow()
    {
        int numberHorizontal = 0;
        int numberVertical = 0;
        Dot firstPiece = findMatches.currentMatches[0].GetComponent<Dot>();
        if (firstPiece != null)
            foreach (var currentPiece in findMatches.currentMatches)
            {
                Dot dot = currentPiece.GetComponent<Dot>();
                if (dot.row == firstPiece.row)
                    numberHorizontal++;
                if (dot.column == firstPiece.column)
                    numberVertical++;
            }
        return (numberVertical == 5 || numberHorizontal == 5);
    }

    private void CheckToMakeBombs()
    {
        if (findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7)
            findMatches.CheckBombs();
        if (findMatches.currentMatches.Count == 5 || findMatches.currentMatches.Count == 8)
            if (ColumnOrRow())
            {
                if (currentDot != null)
                    if (currentDot.isMatched)
                    {
                        if (!currentDot.isColorBomb)
                        {
                            currentDot.isMatched = false;
                            currentDot.MakeColorBomb();
                        }
                    }
                    else
                    {
                        if (currentDot.otherDot != null)
                        {
                            Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if (otherDot.isMatched)
                                if (!otherDot.isColorBomb)
                                {
                                    otherDot.isMatched = false;
                                    otherDot.MakeColorBomb();
                                }
                        }
                    }
            }
            else
            {
                if (currentDot != null)
                    if (currentDot.isMatched)
                        if (!currentDot.isAdjacentBomb)
                        {
                            currentDot.isMatched = false;
                            currentDot.MakeAdjacentBomb();
                        }
                        else
                        {
                            if (currentDot.otherDot != null)
                            {
                                Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                                if (otherDot.isMatched)
                                    if (!otherDot.isAdjacentBomb)
                                    {
                                        otherDot.isMatched = false;
                                        otherDot.MakeAdjacentBomb();
                                    }
                            }
                        }
            }
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if (allDots[column, row].GetComponent<Dot>().isMatched)
        {
            //How many elements are in the matched pieces list from findmatches?
            if (findMatches.currentMatches.Count >= 4)
                CheckToMakeBombs();

            if (breakableTiles[column, row] != null)
            {
                breakableTiles[column, row].TakeDamage(1);
                if (breakableTiles[column, row].hitPoints <= 0)
                    breakableTiles[column, row] = null;
            }

            if (goalManager != null)
            {
                goalManager.CompareGoal(allDots[column, row].tag.ToString());
                goalManager.UpdateGoals();
            }

            if (soundManager != null)
                soundManager.PlayPopSound();
            GameObject particle = Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, .5f);
            Destroy(allDots[column, row]);
            scoreManager.IncreaseScore(basePieceValue * streakValue);
            allDots[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                if (allDots[i, j] != null)
                    DestroyMatchesAt(i, j);
        findMatches.currentMatches.Clear();
        StartCoroutine(DecreaseRowCo());
    }

    private IEnumerator DecreaseRowCo()
    {
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                if (!blankSpaces[i, j] && allDots[i, j] == null)
                    for (int k = j + 1; k < height; k++)
                        if (allDots[i, k] != null)
                        {
                            allDots[i, k].GetComponent<Dot>().row = j;
                            allDots[i, k] = null;
                            break;
                        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                if (allDots[i, j] == null && !blankSpaces[i, j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int dotToUse = Random.Range(0, dotsColor.Length);
                    int maxIterations = 0;
                    while (MatchesAt(i, j, dotsColor[dotToUse]) && maxIterations < 100)
                    {
                        maxIterations++;
                        dotToUse = Random.Range(0, dotsColor.Length);
                    }
                    maxIterations = 0;
                    GameObject piece = Instantiate(dotsColor[dotToUse], tempPosition, Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;
                }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                if (allDots[i, j] != null)
                    if (allDots[i, j].GetComponent<Dot>().isMatched)
                        return true;
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(refillDelay);
        while (MatchesOnBoard())
        {
            streakValue ++;
            DestroyMatches();
            yield return new WaitForSeconds(refillDelay * 2);
        }
        findMatches.currentMatches.Clear();
        currentDot = null;

        yield return new WaitForSeconds(refillDelay);
        currentState = GameState.move;
        streakValue = 1;
    }
}


