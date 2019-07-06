using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PropSpawner : MonoBehaviour
{
	[Tooltip("What prefab to spawn")]
	[SerializeField] GameObject propPrefab;

	[Tooltip("How many prefabs to spawn")]
	[SerializeField] int propCount = 10;
	
	[Tooltip("How far a prefab can spawn from the current position")]
	[SerializeField] float spawnRadius = 3;

	//Spawn an amount of prefabs within a defined radius
	void Start()
	{
		for (int i = 0; i < propCount; i++)
		{
			Instantiate (propPrefab, transform.position + (Random.insideUnitSphere * spawnRadius), Quaternion.identity);
		}
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere (transform.position, spawnRadius);
	}
}