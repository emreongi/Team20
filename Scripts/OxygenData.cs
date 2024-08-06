using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Oxygen", menuName = "ScriptableObject/OxygenData")]

public class OxygenData : ItemData
{
    [Header("Settings")]
    public int value;
}
