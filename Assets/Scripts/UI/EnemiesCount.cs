using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemiesCount : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI enemiesCount;
    public int Enemies { get; private set; } //move to Enemy class
    private TextMeshPro text;

    private void Start()
    {
        Enemies = 15;
        enemiesCount.text = $"Enemies: {Enemies}";
    }

    private void Update()
    {
        enemiesCount.text = $"Enemies: {Enemies}";
    }

    private void OnEnable()
    {
        Enemy.EnemyKilled += SetEnemyCount;
    }

    private void OnDisable()
    {
        Enemy.EnemyKilled -= SetEnemyCount;
    }

    private void SetEnemyCount()
    {
        Enemies--;
    }
}
