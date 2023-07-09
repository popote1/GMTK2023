using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundAtSpawn : MonoBehaviour
{
    public AudioSource AudioSource;

    public AudioClip[] Clips;

    // Start is called before the first frame update
    void Start()
    {
        AudioSource.clip = Clips[Random.Range(0, Clips.Length)];
        AudioSource.Play();
    }
}
