using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverText;
    private EnemiesCount enemiesCount;

    private void Start()
    {
        enemiesCount = FindObjectOfType<EnemiesCount>();
    }
    private void Update()
    {
        if (Player.HitPoints <= 0)
        {
            gameOverPanel.SetActive(true);
        } 
        if (enemiesCount.Enemies <= 0)
        {
            gameOverPanel.SetActive(true);
            gameOverText.text = "You Won!";
        }
    }

    public void Quit()
    {
        UnityEditor.EditorApplication.isPlaying = false;
    }

    public void Restart()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
