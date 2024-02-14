using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource normalAudioSource;
    AudioSource observerAudioSource;
    AudioSource effectAudioSource;

    [SerializeField] AudioClip fireAudioClip;
    [SerializeField] AudioClip shutterAudioClip;
    float currentAudioTime;
    void Awake()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        normalAudioSource = audioSources[0];
        observerAudioSource = audioSources[1];
        effectAudioSource = audioSources[2];
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }    
    
    public void Initialize()
    {
        normalAudioSource.Play();
        currentAudioTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayNormalAudio()
    {
        normalAudioSource.Play();
    }

    public void StopNormalAudio()
    {
        currentAudioTime = normalAudioSource.time;
        normalAudioSource.Stop();
    }

    public void ResumeNormalAudio()
    {
        normalAudioSource.time = currentAudioTime;
        normalAudioSource.pitch = 1 / Player.Gamma;
        normalAudioSource.Play();
    }

    public void PlayObserverAudio()
    {
        observerAudioSource.Play();
    }

    public void StopObserverAudio()
    {
        observerAudioSource.Stop();
    }

    public void PlayFireAudio()
    {
        effectAudioSource.PlayOneShot(fireAudioClip, 0.1f);
    }

    public void StopFireAudio()
    {
        effectAudioSource.Stop();
    }

    public void PlayShutterAudio()
    {
        effectAudioSource.PlayOneShot(shutterAudioClip, 0.1f);
    }
}
