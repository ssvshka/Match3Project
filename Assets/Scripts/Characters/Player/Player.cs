using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static int HitPoints { get; private set; } 

    private void Start()
    {
        HitPoints = 5;
    }
    private void OnTriggerEnter(Collider other)
    {
        HitPoints--;
    }
}
