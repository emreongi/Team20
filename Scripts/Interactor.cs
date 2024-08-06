using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    [SerializeField] float maxInteractingDistance = 250;
    [SerializeField] float interactingRadius = 20;

    Transform playerTransform;
    InputAction interactAction;
    Animator animator;

    public bool interactionRequest = false;

    [HideInInspector] public InteractableObject interactableTarget;

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        playerTransform = player.transform;

        interactAction = GetComponent<PlayerInput>().actions["Interact"];
        interactAction.performed += Interact;

        animator = GetComponent<Animator>();
/*
        // Set up the trigger collider
        SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = interactingRadius;
*/
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            if (other.TryGetComponent<InteractableObject>(out interactableTarget))
            {
                interactionRequest = true;
                interactableTarget.TargetOn();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item") && other.GetComponent<InteractableObject>() == interactableTarget)
        {
            interactableTarget.TargetOff();
            interactableTarget = null;
            animator.SetFloat("Idle", 0f);
            interactionRequest = false;
        }
    }

    private void Interact(InputAction.CallbackContext obj)
    {
        if (interactableTarget != null)
        {
            if (Vector3.Distance(transform.position, interactableTarget.transform.position) <= interactableTarget.interactionDistance)
            {
                interactableTarget.Interact();
                animator.SetFloat("Idle", 1f);
            }
        }
        else
        {
            animator.SetFloat("Idle", 0f);
            print("Etkileşimli nesne yok!");
        }
    }

    private void OnDestroy()
    {
        interactAction.performed -= Interact;
    }
}