using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bullet", menuName = "ScriptableObject/BulletData")]

public class BulletData : ItemData
{
    [Header("Settings")]
    public float speed;
    public float damage;
    public float maxLifeSpan;
    public float responseTime;
    public bool isGravity = false;
    public PhysicMaterial physicMaterial;
    public GameObject explosionObject;
}
