using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickerCharacter : Character
{
	[Tooltip ("How close we need to be to KICK an object")]
	public float ActionRange = 5f;

	[Tooltip("How hard we KICK the object")]
	public float ActionForce = 15f;
	
	/// <summary>
	/// Our action is to KICK rigidbodies we look at
	/// </summary>
    protected override void TryAction()
    {
	    var ray = new Ray(head.position, head.forward);
		if (Physics.Raycast (ray, out var hitInfo, ActionRange))
		{
			if (!hitInfo.rigidbody)
			{
				//No rigidbody, so nothing to KICK
				var selfForce = -ray.direction * ActionForce;
				Rigidbody.AddForce (selfForce, ForceMode.VelocityChange);
				
				//Set LastAirVelocity, so we continue to move in the air
				LastAirVelocity = selfForce.ToVector2 ();
				return;
			}
			
			hitInfo.rigidbody.AddForceAtPosition ((ray.direction + new Vector3(0, 0.2f)) * ActionForce, hitInfo.point, ForceMode.Impulse);
		}
    }
}
