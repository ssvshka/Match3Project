using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float slow = 0.7f;
    public static int firstSpawn = 5;
    public int HitPoints { get; private set; }
    private bool isSlowed;
    public delegate void EnemySpawnDelegate();
    public static event EnemySpawnDelegate EnemyKilled;

    private void OnTriggerEnter(Collider other)
    {
        if (GetComponent<SpriteRenderer>().color == other.GetComponent<SpriteRenderer>().color
         || other.GetComponent<Player>() != null)
        {
            Destroy(gameObject);
            EnemyKilled();
        }
        else if (!isSlowed)
        {
            isSlowed = true;
            speed *= slow;
        }
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
