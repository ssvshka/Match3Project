using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    private void Update()
    {
        GetComponent<TextMeshProUGUI>().text = $"HP: {Player.HitPoints}";
    }
}
