using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeroSounds : MonoBehaviour
{

    private AudioSource audio;
    [SerializeField] private AudioClip[] sounds;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    public void Steps()
    {
        if (HeroStats.Instance.terrain == "grass")
        {
            audio.PlayOneShot(sounds[0]);
        }
        else 
        { 
            audio.PlayOneShot(sounds[1]);
        }
    }

    public void Jump()
    {
        audio.PlayOneShot(sounds[2]);
    }

    public void Slash()
    {
        audio.PlayOneShot(sounds[3]);
    }

    public void Magic()
    {
        audio.PlayOneShot(sounds[4]);
    }

    public void Hit()
    {
        audio.PlayOneShot(sounds[5]);
    }
}
