using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controller : MonoBehaviour
{
    public float speed = 15.0f;
    public bool canJump = true;
    public float jumpHeight = 2.0f;
    public CinemachineFreeLook playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 60.0f;

    bool grounded = false;
    Rigidbody r;
    Vector2 rotation = Vector2.zero;
    float maxVelocityChange = 10.0f;

    private GameSceneUI gameSceneUI;
    void Awake()
    {
        gameSceneUI = GameObject.Find("UIManager").GetComponent<GameSceneUI>();
        GameObject camerafinder = GameObject.Find("FreeLook Camera");
        playerCamera = camerafinder.GetComponent<CinemachineFreeLook>();
        r = GetComponent<Rigidbody>();
        r.freezeRotation = true;
        r.useGravity = false;
        r.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rotation.y = transform.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if(!gameSceneUI.resumePanelOpen)
        {
            // Player and Camera rotation
            rotation.x += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit);


            Quaternion localRotation = Quaternion.Euler(0f, Input.GetAxis("Mouse X") * lookSpeed, 0f);
            transform.rotation = transform.rotation * localRotation;
        }
    }

    void FixedUpdate()
    {
        
        if (grounded)
        {
            // Calculate how fast we should be moving
            Vector3 forwardDir = Vector3.Cross(transform.up, -playerCamera.transform.right).normalized;
            Vector3 rightDir = Vector3.Cross(transform.up, playerCamera.transform.forward).normalized;
            Vector3 targetVelocity = (forwardDir * Input.GetAxis("Vertical") + rightDir * Input.GetAxis("Horizontal")) * speed;

            Vector3 velocity = transform.InverseTransformDirection(r.velocity);
            velocity.y = 0;
            velocity = transform.TransformDirection(velocity);
            Vector3 velocityChange = transform.InverseTransformDirection(targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;
            velocityChange = transform.TransformDirection(velocityChange);

            r.AddForce(velocityChange, ForceMode.VelocityChange);

            if (Input.GetKeyDown(KeyCode.Space) && canJump)
            {
                r.AddForce(transform.up * jumpHeight, ForceMode.VelocityChange);
            }
        }

        grounded = false;
    }

    void OnCollisionStay()
    {
        grounded = true;
    }
}
