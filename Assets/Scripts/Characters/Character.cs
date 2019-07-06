using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent (typeof(Rigidbody), typeof(CapsuleCollider))]
public class Character : MonoBehaviour
{
	protected Controller Controller;

	[Tooltip ("How this character will be controlled")] [SerializeField]
	protected InputControllerType ControllerType;

	[SerializeField] protected Transform head;

	public Transform Head => head;

	[Header ("Stats")] //
	[Tooltip ("Max movement speed")] //
	[SerializeField]
	protected float MoveSpeed = 5f;

	[Tooltip ("How fast we reach max speed")] //
	[SerializeField]
	protected float MoveSharpness = 10f;

	[Tooltip ("Vertical force for jump")] //
	[SerializeField]
	protected float JumpForce = 7f;

	[Tooltip ("Custom gravity scale")] //
	[SerializeField]
	protected float GravityMultipier = 1.2f;

	[Tooltip ("How frequent a footstep event is triggered (for SFX)")] //
	[SerializeField]
	protected float StepLength = 1.4f;

	[Tooltip ("How close to an object we need to be to interact with it")] //
	[SerializeField]
	protected float InteractRange = 4f;

	//Components
	protected internal Rigidbody Rigidbody;
	protected CapsuleCollider Collider;

	//Runtime values
	protected Controller.InputPacket CurrentInput;
	protected Vector3 TargetVelocity;
	protected Vector2 LastAirVelocity;
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

		//Use action input
		HandleActions ();
		HandleFootsteps ();
	}

	void FixedUpdate()
	{
		//Use movement input
		HandleMovement ();
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
		var velocity = Rigidbody.velocity;

		//Set body and head rotations based on inputs
		var rotation = Quaternion.Euler (new Vector3 (0, CurrentInput.LookDirection.eulerAngles.y, 0));
		Rigidbody.MoveRotation (rotation);
		head.localEulerAngles = new Vector3 (CurrentInput.LookDirection.eulerAngles.x, 0, 0);

		//Calculate movement
		var moveInput = Vector2.ClampMagnitude (CurrentInput.MoveAxis, 1);
		var relativeMoveDirection = transform.TransformDirection ((moveInput * MoveSpeed).ToVector3 ());

		//Calculate air movement
		if (!OnGround)
		{
			relativeMoveDirection += LastAirVelocity.ToVector3 ();
			LastAirVelocity = Vector2.ClampMagnitude (relativeMoveDirection.ToVector2 (), MoveSpeed);
		}

		//Calculate velocity
		var desiredVelocity = Vector3.Lerp (velocity, relativeMoveDirection, Time.deltaTime * MoveSharpness);
		var clampedVelocity = Vector2.ClampMagnitude (desiredVelocity.ToVector2 (), MoveSpeed);
		TargetVelocity = clampedVelocity.ToVector3 ();
		TargetVelocity.y = velocity.y;

		//We might change our velocity depending on if we're on the ground or in the air
		OnGround = CheckGround ();

		//Apply gravity
		if (!OnGround)
		{
			TargetVelocity.y += Physics.gravity.y * GravityMultipier * Time.deltaTime;
		}

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
		LastAirVelocity = TargetVelocity.ToVector2 ();
	}

	protected void TryInteract()
	{
		//Create a ray based on our Head's transform
		var ray = new Ray (head.position, head.forward);

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
		var groundCheckResults = Physics.SphereCastAll (origin, radius, Vector3.down, radius * 2, GameManager.Main.Layers.Walkable);

		System.Array.Sort (groundCheckResults, new Utilities.RayHitComparer ());

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