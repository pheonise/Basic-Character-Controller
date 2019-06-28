using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterCharacter : Character
{
	public float ShotPower = 20f;	//How fast the 'shot' is fired
	
	protected override void TryAction()
	{
		//Create a ball, and launch it via Physics
		var ballObj = Instantiate (GameManager.Main.Prefabs.InteractiveBall, Head.position + Head.forward, Head.rotation);
		var ballRb = ballObj.GetComponent<Rigidbody> ();
		ballRb.velocity = Head.forward * ShotPower;
	}
}
