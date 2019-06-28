using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A collection of useful functions
/// </summary>
public static class Utilities
{
	/// <summary>
	/// Destroys a GameObject after 'time' seconds. Extends GameObject to allow any script to easily destroy after time
	/// </summary>
	public static IEnumerator DestroyAfterTime(this GameObject gameObject, float time)
	{
		yield return new WaitForSeconds (time);
		
		//If object pooling, release to pool instead of destroying
		Object.Destroy (gameObject);
	}
	
	/// <summary>
	/// Plays an AudioClip at a specific position, then destroys it when it's done
	/// </summary>
	public static void PlayTempAudio(AudioClip clip, Vector3 position, float pitchVariance = 0)
	{
		if (!GameManager.Main)
		{
			Debug.LogError ("No GameManager in the scene!");
			return;
		}
		
		//Instantiate the TempAudio prefab, and play the clip
		var tempAudioObj = Object.Instantiate (GameManager.Main.Prefabs.TempAudio, position, Quaternion.identity);
		var tempAudio = tempAudioObj.GetComponent<TempAudio> ();
		tempAudio.Play (clip, pitchVariance);
	}

	/// <summary>
	/// Return a pleasant-looking random color
	/// </summary>
	public static Color RandomColor()
	{
		return Color.HSVToRGB (Random.value, Random.Range (0.7f, 0.9f), Random.Range (0.7f, 0.9f));
	}
	
	/// <summary>
	/// Convert an XZ Vector3 to XY Vector2
	/// </summary>
	public static Vector2 ToVector2(this Vector3 input)
	{
		return new Vector2(input.x, input.z);
	}
	
	/// <summary>
	/// Convert an XY Vector2 to XZ Vector3
	/// </summary>
	public static Vector3 ToVector3(this Vector2 input)
	{
		return new Vector3(input.x, 0, input.y);
	}

	/// <summary>
	/// Sort a RaycastHit array by distance, closest-to-furthest
	/// </summary>
	public class RayHitComparer: IComparer
	{
		public int Compare(object x, object y)
		{
			return ((RaycastHit)x).distance.CompareTo(((RaycastHit)y).distance);
		}
	}
}
