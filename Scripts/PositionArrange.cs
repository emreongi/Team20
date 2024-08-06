using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionArrange : MonoBehaviour
{
    private float spacing = 20f;
    // Start is called before the first frame update
    void Start()
    {
        Arrange();
    }
    void OnTransformChildrenChanged()
    {
        Arrange();
    }

    void Arrange()
    {
        // Başlangıç pozisyonunu belirle
        Vector3 startPosition = transform.position;

        // Her bir child objesini sırayla yerleştir
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Vector3 newPosition = startPosition + new Vector3(i * spacing, 0, i * spacing);
            child.position = newPosition;
        }
    }
}
