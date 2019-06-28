using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
	public static CameraController Main;
	public static Camera Camera;		//Static access to our Camera (because Camera.main is expensive)

	public Transform Target;			//The transform we're following
	public float Sensitivity = 1f;		//Multiplier for mouse axis values
	
	public float FollowSpeed = 100f;	//How fast we're following the target (lower is slower)
	public float RotationSpeed = 100f;	//How fast we're matching rotation of the target
	
	Vector3 currentPosition;
	Quaternion currentRotation;
	Vector3 targetForward;
	float verticalAngle;

	float minAngle = -89;	//Less than 90 to avoid gimbal lock
	float maxAngle = 89;

	void Awake()
	{
		Main = this;
		Camera = GetComponent<Camera> ();
	}
	
	public void SetTarget(Transform t)
	{
		Target = t;
		targetForward = Target.forward;
	}

	public void LookInput(Vector2 axis)
	{
		var delta = Time.deltaTime;
		var worldUp = Vector3.up;
		
		//Calculate rotation after input
		Quaternion rotationFromInput = Quaternion.Euler(worldUp * (axis.x * Sensitivity));
		targetForward = rotationFromInput * targetForward;
		targetForward = Vector3.Cross(worldUp, Vector3.Cross(targetForward, worldUp));
		verticalAngle -= axis.y * Sensitivity;
		verticalAngle = Mathf.Clamp(verticalAngle, minAngle, maxAngle);
		
		//Calculate final rotation values		
		Quaternion forwardRotation = Quaternion.LookRotation(targetForward, worldUp);
		Quaternion verticalRotation = Quaternion.Euler(verticalAngle, 0, 0);
		
		currentPosition = Vector3.Lerp(currentPosition, Target.position, FollowSpeed * delta);
		currentRotation = Quaternion.Slerp(transform.rotation, forwardRotation * verticalRotation, RotationSpeed * delta);
		
		transform.SetPositionAndRotation (currentPosition, currentRotation);
	}
}