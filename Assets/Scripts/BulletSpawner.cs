using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private Bullet bullet;
    private void OnEnable()
    {
        PlayerControls.OnSpacePressed += Shoot;
    }

    private void OnDisable()
    {
        PlayerControls.OnSpacePressed -= Shoot;
    }
    
    private void Shoot()
    {
        Instantiate(bullet, transform.position, transform.rotation);
    }
}
