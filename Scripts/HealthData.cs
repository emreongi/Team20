using UnityEngine;

[CreateAssetMenu(fileName = "Health", menuName = "ScriptableObject/HealthData")]

public class HealthData : ItemData
{
    public float healthValue;
    public float duration;
}
