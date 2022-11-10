using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private int hitPoints;

    //public delegate void OnShooting();
    //public static event OnShooting OnShoot;

    private void OnTriggerEnter(Collider other)
    {
        hitPoints--;
    }
}
