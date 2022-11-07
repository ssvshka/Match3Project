using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    private Board board;
    public float cameraOffset;
    public float aspectRatio = .625f;
    public float padding = 2;
    public float yOffset = 1;

    private void Start()
    {
        board = FindObjectOfType<Board>();
        if (board != null)
        {
            RepositionCamera(board.Width - 1, board.Height - 1);
        }
    }

    private void RepositionCamera(float x, float y)
    {
        transform.position = new Vector3(x / 2, y / 2 + yOffset, cameraOffset);
        if (board.Width >= board.Height)
            Camera.main.orthographicSize = (board.Width / 2 + padding) / aspectRatio;
        else
            Camera.main.orthographicSize = board.Height / 2 + padding;
    }
}
