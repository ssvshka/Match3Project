using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    private Board board;
    public float cameraOffset;
    public float aspectRatio = .625f;
    public float padding = 2;

    private void Start()
    {
        board = FindObjectOfType<Board>();
        if (board != null)
        {
            RepositionCamera(board.width - 1, board.height - 1);
        }
    }

    private void RepositionCamera(float x, float y)
    {
        transform.position = new Vector3(x / 2, y / 2, cameraOffset);
        if (board.width >= board.height)
            Camera.main.orthographicSize = (board.width / 2 + padding) / aspectRatio;
        else
            Camera.main.orthographicSize = board.height / 2 + padding;
    }
}
