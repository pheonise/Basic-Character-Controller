using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used for objects with classes that can be interacted with
/// EG/ Buttons, Props, etc
/// </summary>
public interface IInteractable
{
	void Interact();
}

/// <summary>
/// Used for objects with classes that can be damaged or destroyed
/// EG/ Characters, Props, etc
/// </summary>
public interface IDamageable
{
	void TakeDamage(float damage);
	void OnDeath();
}