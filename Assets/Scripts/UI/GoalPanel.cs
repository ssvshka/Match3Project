using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoalPanel : MonoBehaviour
{
    public TextMeshProUGUI text { get; set; }
    public Image image { get; set; }
    public Sprite sprite { get; set; }
    public string str { get; set; }

    private void Start()
    {
        Setup();
    }

    private void Setup()
    {
        //image.sprite = sprite;
        text.text = str;
    }
}

