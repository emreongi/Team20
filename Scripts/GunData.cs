using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "ScriptableObject/GunData")]

public class GunData : ItemData
{
    public bool isAutomotic;
    public Vector3 gunScale;
    public GameObject muzzleEffect;
    public AudioClip fireClip;

    [Header("Reloading")]
    public int currentAmmo;
    public int magSize;
    public float fireRate;
    public float reloadTime;

    [Header("Bullet")]
    public GameObject bulletPrefab;
}
