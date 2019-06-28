using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ColorChanger : MonoBehaviour, IInteractable
{
    Material material;

    void Awake()
    {
        var renderer = GetComponent<Renderer> ();
        material = renderer.material;
        
        //Pick a random color on spawn
        Interact ();
    }

    public void Interact()
    {
        material.color = Utilities.RandomColor ();
    }
}
