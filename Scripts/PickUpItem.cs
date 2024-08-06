using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class PickUpItem : InteractableObject
{
    [Header("Item Data")]
    [SerializeField] string itemName;
    //[SerializeField] Item item;
    //[SerializeField] int Amount = 1;

    public override void Start()
    {
        base.Start();
        //interactableName = itemName.itemName;
        interactableName = itemName;
    }

    protected override void Interaction()
    {
        base.Interaction();
        print("Bu "+ itemName + " çantaya eklendi");
        //Destroy(this.gameObject);
    }
}
