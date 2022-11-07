using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Bullet : MonoBehaviour
{
    [SerializeField] private float forceMultiplier;
    public Color BulletColor { get; set; }
    private Rigidbody rb;
    

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Time.deltaTime * forceMultiplier * transform.up;
    }
}
