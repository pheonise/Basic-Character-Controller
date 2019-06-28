using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandle : MonoBehaviour, IInteractable
{
    bool isOpen;

    Animator animator;
    
    void Awake()
    {
        animator = GetComponentInParent<Animator> ();
    }
    
    public void Interact()
    {
        isOpen = !isOpen;
        animator.SetBool ("IsOpen", isOpen);
    }
}
