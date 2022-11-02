using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoalPanel : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Image image;
    public Sprite sprite;
    public string str;

    private void Start()
    {
        Setup();
    }

    private void Setup()
    {
        image.sprite = sprite;
        text.text = str;
    }
}

