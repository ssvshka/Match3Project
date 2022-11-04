using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int hitPoints;
    private void OnTriggerEnter(Collider other)
    {
        hitPoints--;
    }
}
