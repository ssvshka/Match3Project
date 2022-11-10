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
    public int Width;
    public int Height;

    public GameObject[,] AllDots { get; set; }
    public Dot CurrentDot;
    public int[] ScoreGoals;

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
        blankSpaces = new bool[Width, Height];
        AllDots = new GameObject[Width, Height];
        breakableTiles = new Jelly[Width, Height];
        SetUp();
    }

    private void SetUp()
    {
        GenerateBlankSpaces();
        GenerateBreakableTiles();
        for (int i = 0; i < Width; i++)
            for (int j = 0; j < Height; j++)
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
                    dot.GetComponent<Dot>().Row = j;
                    dot.GetComponent<Dot>().Column = i;
                    dot.transform.parent = this.transform;
                    dot.name = $"({i}, {j})";
                    AllDots[i, j] = dot;
                }
            }
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

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1 && row > 1)
        {
            if (AllDots[column - 1, row] != null && AllDots[column - 2, row] != null)
                if (AllDots[column - 1, row].tag == piece.tag && AllDots[column - 2, row].tag == piece.tag)
                    return true;

                    if (AllDots[column, row - 1] != null && AllDots[column, row - 2] != null)
                if (AllDots[column, row - 1].tag == piece.tag && AllDots[column, row - 2].tag == piece.tag)
                    return true;
        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
                if (AllDots[column, row - 1] != null && AllDots[column, row - 2] != null)
                    if (AllDots[column, row - 1].tag == piece.tag && AllDots[column, row - 2].tag == piece.tag)
                        return true;
            if (column > 1)
                if (AllDots[column - 1, row] != null && AllDots[column - 2, row] != null)
                    if (AllDots[column - 1, row].tag == piece.tag && AllDots[column - 2, row].tag == piece.tag)
                        return true;
        }
        return false;
    }

    private bool ColumnOrRow()
    {
        int numberHorizontal = 0;
        int numberVertical = 0;
        Dot firstPiece = findMatches.CurrentMatches[0].GetComponent<Dot>();
        if (firstPiece != null)
            foreach (var currentPiece in findMatches.CurrentMatches)
            {
                Dot dot = currentPiece.GetComponent<Dot>();
                if (dot.Row == firstPiece.Row)
                    numberHorizontal++;
                if (dot.Column == firstPiece.Column)
                    numberVertical++;
            }
        return (numberVertical == 5 || numberHorizontal == 5);
    }

    private void CheckToMakeBombs()
    {
        if (findMatches.CurrentMatches.Count == 4 || findMatches.CurrentMatches.Count == 7)
            findMatches.CheckBombs();
        if (findMatches.CurrentMatches.Count == 5 || findMatches.CurrentMatches.Count == 8)
            if (ColumnOrRow())
            {
                if (CurrentDot != null)
                    if (CurrentDot.IsMatched)
                    {
                        if (!CurrentDot.IsColorBomb)
                        {
                            CurrentDot.IsMatched = false;
                            CurrentDot.MakeColorBomb();
                        }
                    }
                    else
                    {
                        if (CurrentDot.OtherDot != null)
                        {
                            Dot otherDot = CurrentDot.OtherDot.GetComponent<Dot>();
                            if (otherDot.IsMatched)
                                if (!otherDot.IsColorBomb)
                                {
                                    otherDot.IsMatched = false;
                                    otherDot.MakeColorBomb();
                                }
                        }
                    }
            }
            else
            {
                if (CurrentDot != null)
                    if (CurrentDot.IsMatched)
                        if (!CurrentDot.IsAdjacentBomb)
                        {
                            CurrentDot.IsMatched = false;
                            CurrentDot.MakeAdjacentBomb();
                        }
                        else
                        {
                            if (CurrentDot.OtherDot != null)
                            {
                                Dot otherDot = CurrentDot.OtherDot.GetComponent<Dot>();
                                if (otherDot.IsMatched)
                                    if (!otherDot.IsAdjacentBomb)
                                    {
                                        otherDot.IsMatched = false;
                                        otherDot.MakeAdjacentBomb();
                                    }
                            }
                        }
            }
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if (AllDots[column, row].GetComponent<Dot>().IsMatched)
        {
            //How many elements are in the matched pieces list from findmatches?
            if (findMatches.CurrentMatches.Count >= 4)
                CheckToMakeBombs();

            if (breakableTiles[column, row] != null)
            {
                breakableTiles[column, row].TakeDamage(1);
                if (breakableTiles[column, row].hitPoints <= 0)
                    breakableTiles[column, row] = null;
            }

            if (goalManager != null)
            {
                goalManager.CompareGoal(AllDots[column, row].tag.ToString());
                goalManager.UpdateGoals();
            }

            if (soundManager != null)
                soundManager.PlayPopSound();
            
            GameObject particle = Instantiate(destroyEffect, AllDots[column, row].transform.position, Quaternion.identity);
            
            Destroy(particle, .5f);
            Destroy(AllDots[column, row]);
            scoreManager.IncreaseScore(basePieceValue * streakValue);
            AllDots[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < Width; i++)
            for (int j = 0; j < Height; j++)
                if (AllDots[i, j] != null)
                    DestroyMatchesAt(i, j);
        //var color = GetColor();
        //Bullet.bulletClip.Add(color);
        findMatches.CurrentMatches.Clear();
        StartCoroutine(DecreaseRowCo());
    }

    private IEnumerator DecreaseRowCo()
    {
        for (int i = 0; i < Width; i++)
            for (int j = 0; j < Height; j++)
                if (!blankSpaces[i, j] && AllDots[i, j] == null)
                    for (int k = j + 1; k < Height; k++)
                        if (AllDots[i, k] != null)
                        {
                            AllDots[i, k].GetComponent<Dot>().Row = j;
                            AllDots[i, k] = null;
                            break;
                        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard()
    {
        for (int i = 0; i < Width; i++)
            for (int j = 0; j < Height; j++)
                if (AllDots[i, j] == null && !blankSpaces[i, j])
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
                    AllDots[i, j] = piece;
                    piece.GetComponent<Dot>().Row = j;
                    piece.GetComponent<Dot>().Column = i;
                }
    }

    private bool AreMatchesOnBoard()
    {
        for (int i = 0; i < Width; i++)
            for (int j = 0; j < Height; j++)
                if (AllDots[i, j] != null)
                    if (AllDots[i, j].GetComponent<Dot>().IsMatched)
                        return true;
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(refillDelay);
        while (AreMatchesOnBoard())
        {
            streakValue++;
            DestroyMatches();
            yield return new WaitForSeconds(refillDelay * 2);
        }
        findMatches.CurrentMatches.Clear();
        CurrentDot = null;

        yield return new WaitForSeconds(refillDelay);
        currentState = GameState.move;
        streakValue = 1;
    }
}


