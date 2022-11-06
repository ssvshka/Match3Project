using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed;
    public static int FirstSpawn { get; private set; }
    public int HitPoints { get; private set; }

    private void Awake()
    {
        FirstSpawn = Random.Range(3, 7);
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }

    private void Start()
    {
        if (transform.parent.name == "RightEnemySpawner")
            speed *= -1;
    }
    private void Update()
    {
        transform.Translate(Time.deltaTime * speed * Vector2.right);
    }
}
