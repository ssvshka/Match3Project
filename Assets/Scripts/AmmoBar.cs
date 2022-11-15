using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoBar : MonoBehaviour
{
    private void OnEnable()
    {
        FindMatches.OnMatchesFound += UpdateAmmoUI;
    }

    private void OnDisable()
    {
        FindMatches.OnMatchesFound -= UpdateAmmoUI;
    }

    private void UpdateAmmoUI()
    {
        foreach (var color in Bullet.bulletClip)
        {
            GetComponent<TextMeshProUGUI>().text += $"{Bullet.colorDict[color]} ";
        }
    }
}
