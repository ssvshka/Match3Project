using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    public int Column { get; set; }
    public int Row { get; set; }
    public int PreviousCol { get; private set; }
    public int PreviousRow { get; private set;}
    public int TargetX { get; private set; }
    public int TargetY { get; private set; }
    public bool IsMatched { get; set; }
    
    public float SwipeAngle { get; private set; }
    public float SwipeResist { get; private set; }

    public bool IsColorBomb { get; private set; }
    public bool IsColumnBomb { get; private set; }
    public bool IsRowBomb { get; private set; }
    public bool IsAdjacentBomb { get; private set; }
    public GameObject OtherDot { get; private set; }

    [Header("Powerup Stuff")]
    [SerializeField] private GameObject adjacentBomb;
    [SerializeField] private GameObject rowArrow;
    [SerializeField] private GameObject columnArrow;
    [SerializeField] private GameObject colorBomb;
    private FindMatches findMatches;
    private Board board;
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPosition;

    private void Start()
    {
        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
        SwipeResist = 1f;
    }

    //For tests only
    //private void OnMouseOver()
    //{
    //    if (Input.GetMouseButtonDown(1))
    //    {
    //        IsAdjacentBomb = true;
    //        GameObject color = Instantiate(adjacentBomb, transform.position, Quaternion.identity);
    //        color.transform.parent = this.transform;
    //    }
    //}

    private void Update()
    {
        TargetX = Column;
        TargetY = Row;
        if (Mathf.Abs(TargetX - transform.position.x) > .1)
        {
            //Move Towards The Target
            tempPosition = new Vector2(TargetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (board.AllDots[Column, Row] != this.gameObject)
                board.AllDots[Column, Row] = this.gameObject;
            findMatches.FindAllMatches();
        }
        else
        {
            //Directly Set The Position
            tempPosition = new Vector2(TargetX, transform.position.y);
            transform.position = tempPosition;            
        }
        if (Mathf.Abs(TargetY - transform.position.y) > .1)
        {
            //Move Towards The Target
            tempPosition = new Vector2(transform.position.x, TargetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (board.AllDots[Column, Row] != this.gameObject)
                board.AllDots[Column, Row] = this.gameObject;
            findMatches.FindAllMatches();
        }
        else
        {
            //Directly Set The Position
            tempPosition = new Vector2(transform.position.x, TargetY);
            transform.position = tempPosition;
        }
    }

    public IEnumerator CheckMoveCo()
    {
        if (IsColorBomb)
        {
            findMatches.MatchSameColorPieces(OtherDot.tag);
            IsMatched = true;
        }
        else if (OtherDot.GetComponent<Dot>().IsColorBomb)
        {
            findMatches.MatchSameColorPieces(this.gameObject.tag);
            OtherDot.GetComponent<Dot>().IsMatched = true;
        }
        yield return new WaitForSeconds(.5f);
        if (OtherDot != null)
        {
            if (!IsMatched && !OtherDot.GetComponent<Dot>().IsMatched)
            {
                OtherDot.GetComponent<Dot>().Row = Row;
                OtherDot.GetComponent<Dot>().Column = Column;
                Row = PreviousRow;
                Column = PreviousCol;
                yield return new WaitForSeconds(.5f);
                board.CurrentDot = null;
                board.currentState = GameState.move;
            }
            else
                board.DestroyMatches();            
        }       
    }

    private void OnMouseDown()
    {
        if (board.currentState == GameState.move)        
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        if (board.currentState == GameState.move)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    private void CalculateAngle()
    {
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > SwipeResist
         || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > SwipeResist)
        {
            board.currentState = GameState.wait;
            SwipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y,
                                             finalTouchPosition.x - firstTouchPosition.x)
                                             * 180 / Mathf.PI;
            MovePieces();
            
            board.CurrentDot = this;
        }
        else
            board.currentState = GameState.move;
    }

    private void MovePieces(Vector2 direction)
    {
        OtherDot = board.AllDots[Column + (int)direction.x, Row + (int)direction.y];
        PreviousRow = Row;
        PreviousCol = Column;
        if (OtherDot != null)
        {
            OtherDot.GetComponent<Dot>().Column += -1 * (int)direction.x;
            OtherDot.GetComponent<Dot>().Row += -1 * (int)direction.y;
            Column += (int)direction.x;
            Row += (int)direction.y;
            StartCoroutine(CheckMoveCo());
        }
        else
            board.currentState = GameState.move;
    }
    
    private void MovePieces()
    {
        if (SwipeAngle > -45 && SwipeAngle <= 45 && Column < board.Width - 1)
        {
            MovePieces(Vector2.right);
        }
        else if (SwipeAngle > 45 && SwipeAngle <= 135 && Row < board.Height - 1)
        {
            MovePieces(Vector2.up);
        }
        else if ((SwipeAngle > 135 || SwipeAngle <= -135) && Column > 0)
        {
            MovePieces(Vector2.left);
        }
        else if (SwipeAngle < -45 && SwipeAngle >= -135 && Row > 0)
        {
            MovePieces(Vector2.down);
        }
        else        
            board.currentState = GameState.move;
    }

    public void MakeRowBomb()
    {
        IsRowBomb = true;
        GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
    }

    public void MakeColumnBomb()
    {
        IsColumnBomb = true;
        GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
    }

    public void MakeColorBomb()
    {
        IsColorBomb = true;
        GameObject colorB = Instantiate(colorBomb, transform.position, Quaternion.identity);
        colorB.transform.parent = this.transform;
        this.gameObject.tag = "Color";
    }

    public void MakeAdjacentBomb()
    {
        IsAdjacentBomb = true;
        GameObject adjacent = Instantiate(adjacentBomb, transform.position, Quaternion.identity);
        adjacent.transform.parent = this.transform;
    }
}
