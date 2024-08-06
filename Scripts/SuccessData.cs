using UnityEngine;

[CreateAssetMenu(fileName = "Success", menuName = "ScriptableObject/SuccessData")]

public class SuccessData : ScriptableObject
{
    public new string name;
    public string successDescription;
    public int successCount;
    public int successValue;
    public Sprite successImage;
}
