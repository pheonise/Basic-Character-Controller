using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TempAudio : MonoBehaviour
{
    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource> ();
    }

    /// <summary>
    /// Plays an audio clip, then destroys it afterwards
    /// PitchVariance allows the sound to vary the pitch, to avoid repetitious sounds
    /// </summary>
    public void Play(AudioClip clip, float pitchVariance = 0)
    {
        if (pitchVariance > 0)
        {
            audioSource.pitch = 1f + Random.Range (-pitchVariance, pitchVariance);
        }
        else
        {
            audioSource.pitch = 1f;
        }
        
        audioSource.clip = clip;
        audioSource.Play();

        //See Utilities.DestroyAfterTime
        StartCoroutine(gameObject.DestroyAfterTime (clip.length + 0.5f));
    }
}
