using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class Character : MonoBehaviour
{
	public Controller Controller;
	public InputControllerType ControllerType;

	public Transform Head;

	[Header("Stats")] //
	public float MoveSpeed = 5f;
	public float MoveSharpness = 10f;
	public float JumpForce = 10f;
	public float GravityMultipier = 2f;
	public float StepLength = 0.4f;
	public float InteractRange = 3f;
	
	protected Rigidbody Rigidbody;
	protected CapsuleCollider Collider;
	protected Controller.InputPacket CurrentInput;

	protected Vector3 TargetVelocity;
	protected bool OnGround;

	protected float DistanceSinceLastStep;

	void Awake()
	{
		//Get components before we do anything else
		AssignComponents ();
	}
	
	void Start()
	{
		//Create the controller
		SetController ();
	}

	void Update()
	{
		//Get input for this frame
		CurrentInput = Controller.GetInput ();
		
		//Use that input
		HandleMovement ();
		HandleActions ();
		HandleFootsteps ();
	}

	//Assign any components that might be accessed
	void AssignComponents()
	{
		Rigidbody = GetComponent<Rigidbody> ();
		Collider = GetComponent<CapsuleCollider> ();
	}
	
	//Create and initialize the Input Controller
	void SetController()
	{
		switch (ControllerType)
		{
			case InputControllerType.Player:
				Controller = new PlayerController ();
				break;
			default:
				Controller = new AIController ();
				break;
		}
		
		//Initialize the controller with this character
		Controller.Init (this);
	}

	protected void HandleMovement()
	{
		//Set body and head rotations based on inputs
		var rotation = Quaternion.Euler (new Vector3 (0, CurrentInput.LookDirection.eulerAngles.y, 0));
		Rigidbody.MoveRotation (rotation);

		Head.localEulerAngles = new Vector3(CurrentInput.LookDirection.eulerAngles.x, 0, 0);
		
		//Calculate movement
		//Clamp movement in Character, so we don't need to do it in every individual Controller
		var moveInput = Vector2.ClampMagnitude (CurrentInput.MoveAxis, 1);
		
		//Calculate our target xz velocity, and match our y velocity
		TargetVelocity = Vector3.Lerp(TargetVelocity, transform.TransformDirection((moveInput * MoveSpeed).ToVector3 ()), MoveSharpness * Time.deltaTime);
		TargetVelocity.y = Rigidbody.velocity.y;
		
		//We might change our velocity depending on if we're on the ground or in the air
		OnGround = CheckGround ();

		//Apply gravity
		TargetVelocity.y += Physics.gravity.y * GravityMultipier * Time.fixedDeltaTime;

		//Jump will set TargetVelocity.y if able
		if (CurrentInput.Jump)
		{
			TryJump ();
		}

		//Change friction depending on movement state/amount
		Collider.material = (OnGround && moveInput.magnitude < 0.01f) ? GameManager.Main.PhysicsMaterials.MaxFriction : GameManager.Main.PhysicsMaterials.ZeroFriction;

		//Set the actual rigidbody's velocity
		Rigidbody.velocity = TargetVelocity;
	}

	//Call the functions associated with the action inputs
	protected void HandleActions()
	{
		if (CurrentInput.Interact)
		{
			TryInteract ();
		}

		if (CurrentInput.Action)
		{
			TryAction ();
		}
	}

	//Play a sound effect every xDistance
	protected void HandleFootsteps()
	{
		if (!OnGround)
		{
			DistanceSinceLastStep = StepLength;
			return;
		}

		//Only care about xz planar movement (y is affected by gravity, which can cause steps even when standing still)
		DistanceSinceLastStep += Rigidbody.velocity.ToVector2 ().magnitude * Time.deltaTime;
		if (DistanceSinceLastStep > StepLength)
		{
			DistanceSinceLastStep = 0;
			Utilities.PlayTempAudio (GameManager.Main.AudioClips.FootStep, transform.position, 0.075f);
		}
	}

	protected void TryJump()
	{
		//Can't jump while in the air
		if (!OnGround)
		{
			//TODO: Maybe we can, if we can Double jump?
			return;
		}
		
		//Just set the velocity, rather than adding a force
		TargetVelocity.y = JumpForce;
	}

	protected void TryInteract()
	{

		//Create a ray based on our Head's transform
		var ray = new Ray(Head.position, Head.forward);
		
		//Alternatively, if only the player can interact, you could create a ray from the camera instead
		//var ray = CameraController.Camera.ViewportPointToRay (new Vector2 (0.5f, 0.5f));

		if (Physics.Raycast (ray, out var hitInfo, InteractRange))
		{
			var interactive = hitInfo.collider.GetComponent<IInteractable> ();
			if (interactive == null)
			{
				//No interactive component
				return;
			}
			
			interactive.Interact ();
		}
	}

	protected virtual void TryAction()
	{
		//Override this on an inheriting class (see ShooterCharacter and KickerCharacter for examples)
	}
	
	//Returns true if on ground, false if not
	bool CheckGround()
	{
		var origin = transform.position + new Vector3 (0, Collider.radius, 0);
		var radius = 0.29f;
		var groundCheckResults = Physics.SphereCastAll(origin, radius, Vector3.down, radius * 2, GameManager.Main.Layers.Walkable);

		System.Array.Sort (groundCheckResults, new Utilities.RayHitComparer());

		for (int i = 0; i < groundCheckResults.Length; i++)
		{
			if (groundCheckResults[i].rigidbody == Rigidbody)
			{
				//Ignore collision with self
				continue;
			}

			//Quick-n-dirty ground-distance check
			if (Mathf.Abs (groundCheckResults[i].point.y - transform.position.y) > 0.05f)
			{
				//Too far from ground
				return false;
			}
			
			//TODO: Do some slope detection here
			
			//Snap to the ground
			var pos = Rigidbody.position;
			pos.y = groundCheckResults[i].point.y;
			Rigidbody.position = (pos);

			TargetVelocity.y = 0;
			return true;
		}

		//Either self-collision, or no collisions at all
		return false;
	}

	//Determines the type of Controller created
	public enum InputControllerType
	{
		None,
		AI,
		Player,
	}
}