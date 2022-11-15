using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float forceMultiplier;
    [SerializeField] private Color[] bulletColorTypes;

    private SpriteRenderer sprite;
    private Rigidbody rb;
    public static Dictionary<Color, int> colorDict = new Dictionary<Color, int>()
    {
        { Color.blue, 0 },
        { Color.green, 0 },
        { new Color(1.0f, 0.3843138f, 0.0f), 0 },
        { Color.magenta, 0 },
        { Color.red, 0 },
        { Color.yellow, 0 },
    };
    public static Queue<Color> bulletClip = new Queue<Color>();

    private FindMatches findMatches;

    private void Start()
    {
        findMatches = FindObjectOfType<FindMatches>();
    }

    private void OnEnable()
    {
        FindMatches.OnMatchesFound += GetColors;
        FindMatches.OnMatchesFound += AddColors;
    }

    private void OnDisable()
    {
        FindMatches.OnMatchesFound -= GetColors;
        FindMatches.OnMatchesFound -= AddColors;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<SpriteRenderer>().color == sprite.color)
            Destroy(gameObject);
        else
            other.GetComponent<Rigidbody>().drag = 0.2f;
    }

    private void AddColors()
    {
        foreach (var color in bulletClip)
            colorDict[color]++;
    }

    private void GetColors()
    {
        var color = findMatches.CurrentMatches[0].tag.Split(' ')[0];
        switch (color)
        {
            case "Blue":
                bulletClip.Enqueue(Color.blue);
                break;
            case "Green":
                bulletClip.Enqueue(Color.green);
                break;
            case "Orange":
                bulletClip.Enqueue(new Color(1.0f, 0.3843138f, 0.0f));
                break;
            case "Purple":
                bulletClip.Enqueue(Color.magenta);
                break;
            case "Red":
                bulletClip.Enqueue(Color.red);
                break;
            case "Light":
                bulletClip.Enqueue(Color.yellow);
                break;
            default:
                break;
        }
    }
}
