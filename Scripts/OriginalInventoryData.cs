using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OriginalInventory", menuName = "ScriptableObject/OriginalInventoryData")]

public class OriginalInventoryData : ScriptableObject
{
    public List<Slot> slots = new List<Slot>();
}
