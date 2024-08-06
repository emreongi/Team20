using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(planetGravity))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
[RequireComponent(typeof(PhotonRigidbodyView))]
[RequireComponent(typeof(PlayerCameraController))]
[RequireComponent(typeof(OffScreenPlayerIndicator))]
[RequireComponent(typeof(JsonManager))]

public class PlayerController : MonoBehaviourPun
{
    private characterAnimation characterAnim; // `characterAnimation` scriptini referans alın

    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float runSpeed = 15f;
    [SerializeField] private float lookSpeed = 2f;
    private float currentSpeed;

    private Vector3 velocityChange;
    private Vector2 rotation = Vector2.zero;
    public GameObject playerFreeLookrCamera;
    [SerializeField] private string cameraName = "FreeLook Camera";
    [SerializeField] private LoggingManager loggingManager;
    
    private SuccessManager successManager;

    void Awake()
    {
        characterAnim = GetComponent<characterAnimation>(); // `characterAnimation` scriptini alın

        GameObject gameManagerObject = GameObject.Find("GameManager");
        if (gameManagerObject != null)
        {
            loggingManager = gameManagerObject.GetComponent<LoggingManager>();
            if (loggingManager == null)
            {
                Debug.LogError("LoggingManager component not found on GameManager.");
            }

            successManager = gameManagerObject.GetComponent<SuccessManager>();
        }
        else
        {
            Debug.LogError("GameManager object not found.");
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Start()
    {
        currentSpeed = walkSpeed;

        if (photonView.IsMine)
        {
            playerFreeLookrCamera = GameObject.Find(cameraName);
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        HandleMovement();
        HandleRotation();

        currentSpeed = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) ? runSpeed : walkSpeed;
    }

    private void HandleMovement()
    {
        Vector3 forwardDir = Vector3.Cross(transform.up, -playerFreeLookrCamera.transform.right).normalized;
        Vector3 rightDir = Vector3.Cross(transform.up, playerFreeLookrCamera.transform.forward).normalized;
        Vector3 targetVelocity = (forwardDir * Input.GetAxis("Vertical") + rightDir * Input.GetAxis("Horizontal")) * currentSpeed;

        // Hareketi transform pozisyonunu doğrudan değiştirerek yapıyoruz
        transform.position += targetVelocity * Time.deltaTime;

       
    }

    private void HandleRotation()
    {
        Quaternion localRotation = Quaternion.Euler(0f, Input.GetAxis("Mouse X") * lookSpeed, 0f);
        transform.rotation = transform.rotation * localRotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //
    }

    private void OnCollisionExit(Collision collision)
    {
        //
    }

    void OnTriggerEnter(Collider collider)
    {
        if (!photonView.IsMine) return;

        if (collider.TryGetComponent<LoggingTrigger>(out LoggingTrigger loggingTrigger))
        {
            loggingManager.loggingTrigger = loggingTrigger;
            loggingManager.triggerObject = collider.gameObject;
            loggingManager.TriggerObjectDetection();
        }
    }
}