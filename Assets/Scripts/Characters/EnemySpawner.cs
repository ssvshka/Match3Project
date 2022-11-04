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
        nextSpawnTime = Enemy.FirstSpawn;
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
    
    private Color SetEnemyColor() => enemyColorTypes[Random.Range(0, enemyColorTypes.Length - 1)];

    private void SpawnColoredEnemy()
    {
        var enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity, this.transform);
        enemy.GetComponent<SpriteRenderer>().color = SetEnemyColor();
    }

    private void SetRandomRespawnRate()
    {
        respawnRate = Random.Range(2, 6);
    }
}
