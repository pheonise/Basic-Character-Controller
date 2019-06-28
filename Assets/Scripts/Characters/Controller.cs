using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class Controller
{
	public Character Character;

	/// <summary>
	/// Set the Character, and handle any extra initialization here
	/// </summary>
	public virtual void Init(Character c)
	{
		Character = c;
	}

	/// <summary>
	/// Returns an InputPacket to the Character every frame
	/// </summary>
	public virtual InputPacket GetInput()
	{
		return new InputPacket ();
	}
	
	//Holds all possible inputs for a Character
	public struct InputPacket
	{
		public Vector2 MoveAxis;
		public Quaternion LookDirection;

		public bool Jump;
		public bool Interact;
		public bool Action;
	}
}

/// <summary>
/// For AI-controlled characters, using NavMeshAgents to determine where to move
/// </summary>
public class AIController : Controller
{
	NavMeshAgent agent;
	
	float timeToNextPoint;

	public override void Init(Character character)
	{
		base.Init(character);
		
		//Add a NavMeshAgent, and give it some good settings
		agent = Character.gameObject.AddComponent<NavMeshAgent>();
		agent.baseOffset = -0.08f;		//Don't hover off the ground
		agent.acceleration = 0;			//We don't want the agent to physically move us, only tell us where to go
		agent.stoppingDistance = 0.5f;
		agent.speed = 1000;
		agent.angularSpeed = 1000;
	}
	
	public override InputPacket GetInput()
	{
		//Calculate a new random destination every 1-3 seconds
		if (timeToNextPoint > 0f)
		{
			timeToNextPoint -= Time.deltaTime;
		}
		else
		{
			agent.SetDestination((Random.insideUnitCircle * 20f).ToVector3());
			timeToNextPoint = Random.Range(1f, 3f);
		}
		
		//Direction relative to ourselves as move input
		var worldDirection = Character.transform.InverseTransformDirection(agent.desiredVelocity);
		return new InputPacket()
		{
			MoveAxis = worldDirection.ToVector2(),
			LookDirection = Quaternion.LookRotation(agent.destination - Character.Head.position),
		};
	}
}

/// <summary>
/// For Player-controlled characters, using keyboard/mouse/controller inputs
/// </summary>
public class PlayerController : Controller
{
	//Inputs defined here as strings, to keep things neatly together
	public const string MouseXInput = "Mouse X";
	public const string MouseYInput = "Mouse Y";
	public const string HorizontalInput = "Horizontal";
	public const string VerticalInput = "Vertical";
	public const string JumpInput = "Jump";
	public const string ActionInput = "Fire1";
	public const string InteractInput = "Fire2";

	public override void Init(Character c)
	{
		base.Init (c);
		
		Cursor.lockState = CursorLockMode.Locked;

		// Tell camera to follow transform
		CameraController.Main.SetTarget(Character.Head);

		//Hide our own meshes, so we can still see clearly
		var meshRenderers = Character.GetComponentsInChildren<MeshRenderer> ();
		foreach (var r in meshRenderers)
		{
			//Change to ShadowsOnly to hide the mesh, but still show the shadows
			//(Rather than just disabling the mesh renderer)
			r.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
		}
	}

	public override InputPacket GetInput()
	{
		// Create the look input vector for the camera
		float mouseLookAxisUp = Input.GetAxisRaw(MouseYInput);
		float mouseLookAxisRight = Input.GetAxisRaw(MouseXInput);
		Vector2 lookInputVector = new Vector2(mouseLookAxisRight, mouseLookAxisUp);

		// Apply inputs to the camera
		CameraController.Main.LookInput (lookInputVector);

		return new InputPacket
		{
			MoveAxis = new Vector2(Input.GetAxisRaw(HorizontalInput), Input.GetAxisRaw(VerticalInput)),
			LookDirection = CameraController.Main.transform.rotation,
			Interact = Input.GetButtonDown (InteractInput),
			Action = Input.GetButtonDown (ActionInput),
			Jump = Input.GetButtonDown(JumpInput),
		};
	}
}
