using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planetGravity : MonoBehaviour
{
    private Transform planet;
    [SerializeField] private const string planetName = "Planet";
    [SerializeField] private bool alignToPlanet = true;

    [SerializeField] private float gravityConstant = 9.8f;
    private float gravityAdjuster = 70.22f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;

        planet = GameObject.FindWithTag(planetName).transform;

        gravityConstant = gravityAdjuster * gravityConstant;
    }

    void FixedUpdate()
    {
        Vector3 toCenter = planet.position - transform.position;
        toCenter.Normalize();


        rb.AddForce(toCenter * gravityConstant, ForceMode.Acceleration);

        if (alignToPlanet)
        {
            Quaternion q = Quaternion.FromToRotation(transform.up, -toCenter);
            q = q * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, q, 1);
        }
    }
}
