using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractableNameText : MonoBehaviour
{
    TextMeshProUGUI text;
    Transform cameraTransform;
    private void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        cameraTransform = Camera.main.transform;
        HideText();
    }

    public void ShowText(InteractableObject interactable)
    {
        if (interactable is PickUpItem)
        {
            text.text = interactable.interactableName + "\n [E] Topla";
        }
        else
        {
            text.text = interactable.interactableName;
        }
        /*else if(interactable is InteractableChest)
        {
            if (((InteractableChest)interactable).isOpen)
            {
                text.text = interactable.interactableName + "\n [E] Kapat";
            }
            else
            {
                text.text = interactable.interactableName + "\n [E] Aç";
            }
        }*/
    }

    public void HideText()
    {
        text.text = "";
    }

    public void SetInteractableNamePosition(InteractableObject interactable)
    {
        if (interactable.TryGetComponent(out BoxCollider boxCollider))
        {
            transform.position = interactable.transform.position + Vector3.up * boxCollider.bounds.size.y;
            transform.LookAt(2 * transform.position - cameraTransform.position);
        }
        else if (interactable.TryGetComponent(out CapsuleCollider capsCollider))
        {
            transform.position = interactable.transform.position + Vector3.up * capsCollider.height;
            transform.LookAt(2 * transform.position - cameraTransform.position);
        }
        else
        {
            print("Collider bulunamadı!");
        }
        
    }
}
