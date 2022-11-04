using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource popSound;
    public AudioSource[] musicChoice;

    public void PlayPopSound() => popSound.Play();

    private void Awake()
    {
        musicChoice[Random.Range(0, musicChoice.Length)].Play();
    }
}
