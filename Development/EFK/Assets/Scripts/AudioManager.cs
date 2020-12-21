using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Sound[] sounds;

    private void Awake()
    {
        foreach (var s in sounds)
        {
            s.SetUpSound(gameObject.AddComponent<AudioSource>());
        }
    }

    private void Start()
    {
        Play("Theme");
    }

    public void Play(string soundName)
    {
        Sound sound = Array.Find(sounds, x => string.Equals(x.Name, soundName));
        if (sound == null) Debug.LogWarning("The audio source is not present");
        else
        {
            sound.Play();
        }
    }
}
