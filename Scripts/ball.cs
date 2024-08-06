using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ball : MonoBehaviour
{
    public Transform gravityTarget;
    public float power = 15000f; 
    public float torque = 500f;
    public float gravity = 20;

    public bool autoOrient = false;
    public float autoOrientSpeed = 1f;
    
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        ProcessInput();
        ProcessGravity();
    }

    void ProcessInput()
    {
        float vt = Input.GetAxis("Vertical");
        Vector3 force = new Vector3(0f,0f, vt * power);
        rb.AddRelativeForce(force);
        
        float hz = Input.GetAxis("Horizontal");
        Vector3 rforce = new Vector3(0f,hz*torque,0f);
        rb.AddRelativeForce(rforce);
    }

    void ProcessGravity()
    {
        Vector3 diff = transform.position - gravityTarget.position;
        rb.AddForce(-diff.normalized * gravity * (rb.mass));

        if (autoOrient)
        {
            AutoOrient(-diff);
        }
    }

    void AutoOrient(Vector3 down)
    {
        Quaternion orientationDirection = Quaternion.FromToRotation(-transform.up, down) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, orientationDirection, autoOrientSpeed * Time.deltaTime);
    }
}
    