using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    [SerializeField] private string name;
    [SerializeField] private AudioClip clip;
    [Range(0f,1f)]
    [SerializeField] private float volume;
    [Range(.1f,3f)]
    [SerializeField] private float pitch = 1;
    [SerializeField] private bool loop;
    [Range(0f,1f)]
    [SerializeField] private float spatialBlend;
    [SerializeField] private AudioMixerGroup audioMixer;
    [SerializeField] private float maxDistance;
    private AudioSource source;
    public string Name => name;
    public void SetUpSound(AudioSource audioSource)
    {
        source = audioSource;
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.loop = loop;
        source.spatialBlend = spatialBlend;
        source.outputAudioMixerGroup = audioMixer;
        source.maxDistance = maxDistance;
    }

    public void Play()
    {
        source.Play();
    }
}
