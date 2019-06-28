using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Main;
	
	public GameLayers Layers;
	public GamePrefabs Prefabs;
	public GameAudioClips AudioClips;
	public GamePhysicsMaterials PhysicsMaterials;

	void Awake()
	{
		Main = this;
	}
	
	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Escape))
		{
			Application.Quit ();
		}
	}
	
	//Custom structs to hold things nicely
	[System.Serializable]
	public struct GameLayers
	{
		public LayerMask Walkable;
	}

	//Custom structs to hold things nicely
	[System.Serializable]
	public struct GamePrefabs
	{
		public GameObject TempAudio;
		public GameObject InteractiveBall;
	}

	[System.Serializable]
	public struct GameAudioClips
	{
		//Quick random footstep sound
		public AudioClip FootStep
		{
			get
			{
				var r = Random.Range (0, FootSteps.Length);
				return FootSteps[r];
			}
		}
		
		public AudioClip[] FootSteps;
	}

	[System.Serializable]
	public struct GamePhysicsMaterials
	{
		public PhysicMaterial ZeroFriction;
		public PhysicMaterial MaxFriction;
	}
}