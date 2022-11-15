using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Color[] enemyColorTypes;
    [SerializeField] private GameObject enemyPrefab;
    public float respawnRate { get; private set; }
    private float nextSpawnTime;

    private void Start()
    {
        nextSpawnTime = Enemy.firstSpawn;
    }
    private void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnColoredEnemy();
            SetRandomRespawnRate();
            nextSpawnTime = Time.time + respawnRate;
        }
    }
    
    private void SpawnColoredEnemy()
    {
        var colorNumber = Random.Range(0, enemyColorTypes.Length - 1);
        var color = enemyColorTypes[colorNumber];
        var enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity, this.transform);
        enemy.GetComponent<SpriteRenderer>().color = color;
        SetTagOnEnemy(colorNumber, enemy);
    }

    private void SetRandomRespawnRate()
    {
        respawnRate = Random.Range(6, 10);
    }

    private void SetTagOnEnemy(int number, GameObject enemy)
    {
        switch (number)
        {
            case 0:
                enemy.tag = "Blue Dot";
                break;
            case 1:
                enemy.tag = "Green Dot";
                break;
            case 2:
                enemy.tag = "Orange Dot";
                break;
            case 3:
                enemy.tag = "Purple Dot";
                break;
            case 4:
                enemy.tag = "Red Dot";
                break;
            case 5:
                enemy.tag = "Light Blue Dot";
                break;
            default:
                break;
        }
    }
}
