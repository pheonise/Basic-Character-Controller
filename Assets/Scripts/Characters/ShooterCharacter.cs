using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterCharacter : Character
{
	[Tooltip("How fast the SHOT is launched")]
	public float ShotPower = 20f;
	
	protected override void TryAction()
	{
		//Create a ball, and launch it via Physics
		var ballObj = Instantiate (GameManager.Main.Prefabs.InteractiveBall, head.position + head.forward, head.rotation);
		var ballRb = ballObj.GetComponent<Rigidbody> ();
		ballRb.velocity = head.forward * ShotPower;
	}
}
