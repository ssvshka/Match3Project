using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private int hitPoints;
    [SerializeField] private GameObject[] noses;
    [SerializeField] private Bullet[] bullets;

    private void OnTriggerEnter(Collider other)
    {
        hitPoints--;
    }
}
