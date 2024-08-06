using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObject/ItemData")]

public class ItemData : ScriptableObject
{
    [Header("Info")]
    public new string name;
    public string type;

    [Header("General Features")]
    public bool stackable;
    public Sprite icon;
    public GameObject itemPrefab;
}
