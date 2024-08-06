using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundPlanet : MonoBehaviour
{
    public Transform planet;
    private float rotationSpeed = 2f;

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(planet.position, Vector3.up, rotationSpeed * Time.deltaTime);
        transform.LookAt(planet);
    }
}
