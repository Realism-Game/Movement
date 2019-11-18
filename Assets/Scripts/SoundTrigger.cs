using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SoundTrigger : MonoBehaviour
{
    public AudioClip impact;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter()
    {
        audioSource.Play();
    }
}
