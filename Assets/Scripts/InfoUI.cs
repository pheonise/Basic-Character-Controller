using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoUI : MonoBehaviour
{
	Text infoText;

	void Awake()
	{
		infoText = GetComponent<Text> ();
	}

	void Start()
	{
		//Show some important controls for the demo
		infoText.text = "<b>CONTROLS:</b>\n" +
		                "Movement: <i><b>WASD</b></i>\n" +
		                $"Action: <i><b>{PlayerController.ActionInput}</b></i>\n" +
		                $"Interact: <i><b>{PlayerController.InteractInput}</b></i>";
	}
}