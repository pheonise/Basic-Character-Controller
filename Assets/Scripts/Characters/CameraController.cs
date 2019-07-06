using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
	public static CameraController Main;
	public static Camera Camera;		//Static access to our Camera (because Camera.main is expensive)

	Transform target;			//The transform we're following
	
	[Tooltip("How much to multiply the mouse axis values")]
	[SerializeField] float sensitivity = 1f;		//Multiplier for mouse axis values

	[Tooltip("How fast the camera follows the target's position")]
	[SerializeField] float followSpeed = 50f;	//How fast we're following the target (lower is slower)
	
	[Tooltip("How fast the camera follows the target's rotation")]
	[SerializeField] float rotationSpeed = 50f;	//How fast we're matching rotation of the target
	
	[Tooltip("How far up we can look")]
	[SerializeField] float minAngle = -89;	//Less than 90 to avoid gimbal lock

	[Tooltip("How far down we can look")]
	[SerializeField] float maxAngle = 89;
	
	//Runtime values
	Vector2 lookAxis;
	Vector3 currentPosition;
	Quaternion currentRotation;
	Vector3 targetForward;
	float verticalAngle;

	void Awake()
	{
		Main = this;
		Camera = GetComponent<Camera> ();
	}
	
	public void SetTarget(Transform t)
	{
		target = t;
		targetForward = target.forward;
		
		transform.SetPositionAndRotation (target.position, target.rotation);
	}

	public void LookInput(Vector2 axis)
	{
		lookAxis = axis;
	}
	
	void LateUpdate()
	{
		var delta = Time.deltaTime;
		var worldUp = Vector3.up;

		//Calculate rotation after input
		Quaternion rotationFromInput = Quaternion.Euler(worldUp * (lookAxis.x * sensitivity));
		targetForward = rotationFromInput * targetForward;
		targetForward = Vector3.Cross(worldUp, Vector3.Cross(targetForward, worldUp));
		verticalAngle -= lookAxis.y * sensitivity;
		verticalAngle = Mathf.Clamp(verticalAngle, minAngle, maxAngle);
		
		//Calculate final rotation values		
		Quaternion forwardRotation = Quaternion.LookRotation(targetForward, worldUp);
		Quaternion verticalRotation = Quaternion.Euler(verticalAngle, 0, 0);
		
		currentPosition = Vector3.Lerp (currentPosition, target.position, followSpeed * delta);
		currentRotation = Quaternion.Slerp(currentRotation, forwardRotation * verticalRotation, rotationSpeed * delta);

		transform.SetPositionAndRotation (currentPosition, currentRotation);
	}
}