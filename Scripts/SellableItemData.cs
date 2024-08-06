using UnityEngine.UI;
using UnityEngine;

[CreateAssetMenu(fileName = "SellableItem", menuName = "ScriptableObject/SellableItemData")]

public class SellableItemData : ScriptableObject
{
    public new string name;
    public int id;
    public Type type;
    public int itemPrice;
    public Sprite itemImage;
    public Material itemMaterial;
    public int ranking;
    public bool isPurchasing = false;
}

[System.Serializable]
public enum Type
{
    M416,
    RP500
}
