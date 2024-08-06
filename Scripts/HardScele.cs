using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardScele : MonoBehaviour
{
    [SerializeField] private Vector3 scale;
    void Start()
    {
        transform.localScale = scale;
    }
}
