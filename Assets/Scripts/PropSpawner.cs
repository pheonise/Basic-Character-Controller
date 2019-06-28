using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PropSpawner : MonoBehaviour
{
	[SerializeField] GameObject propPrefab;
	[SerializeField] int propCount = 10;
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