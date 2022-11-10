using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float forceMultiplier;
    [SerializeField] private Color[] bulletColorTypes;

    private Board board;
    private FindMatches findMatches;
    private Color clr;
    private SpriteRenderer sprite;
    private Rigidbody rb;
    public static Color DotColor { get; set; }

    public static Queue<Color> bulletClip = new Queue<Color>();

    private void Start()
    {
        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        sprite = GetComponent<SpriteRenderer>();
        if (bulletClip.Count > 0)
        {
            sprite.color = bulletClip.First();
            bulletClip.Dequeue();
        }
        else
            sprite.color = Color.white;
        
        rb.velocity = Time.deltaTime * forceMultiplier * transform.up;
    }
}
