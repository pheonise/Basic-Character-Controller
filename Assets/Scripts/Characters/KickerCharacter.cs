using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickerCharacter : Character
{
	public float ActionRange = 5f;		//How far we KICK
	public float ActionForce = 15f;		//How hard we KICK
	
	/// <summary>
	/// Our action is to KICK rigidbodies we look at
	/// </summary>
    protected override void TryAction()
    {
	    var ray = new Ray(Head.position, Head.forward);
		if (Physics.Raycast (ray, out var hitInfo, ActionRange))
		{
			if (!hitInfo.rigidbody)
			{
				//No rigidbody, so nothing to KICK
				return;
			}
			
			hitInfo.rigidbody.AddForceAtPosition (ray.direction * ActionForce, hitInfo.point, ForceMode.Impulse);
		}
    }
}
