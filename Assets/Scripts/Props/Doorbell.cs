using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Doorbell : MonoBehaviour, IInteractable
{
	AudioSource audioSource;

	void Awake()
	{
		audioSource = GetComponent<AudioSource> ();
	}

	public void Interact()
	{
//		if (audioSource.isPlaying)
//		{
//			audioSource.Stop ();
//		}
		
		audioSource.Play();
	}
}