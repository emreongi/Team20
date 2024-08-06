using Photon.Pun;
using UnityEngine;

public class characterAnimation : MonoBehaviourPunCallbacks
{
    private Animator _animator;

    public bool isGrounded { get; set; } // `isGrounded` değişkenini public olarak tanımla

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            HandleMovementInput();
        }
    }

    private void HandleMovementInput()
    {
        // Yürüme ve koşma
        if (Input.GetKey(KeyCode.W))
        {
            SetAnimatorFloat("Speed", Input.GetKey(KeyCode.LeftShift) ? 1f : 0.3f);
        }
        else
        {
            SetAnimatorFloat("Speed", 0f);
        }

        // Zıplama
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            SetAnimatorBool("havadaMi", true);
        }
        else if (isGrounded)
        {
            SetAnimatorBool("havadaMi", false);
        }

        // Geri gitme
        if (Input.GetKey(KeyCode.S))
        {
            SetAnimatorBool("backMovement", true);
            SetAnimatorBool("backIdle", true);
        }
        else
        {
            SetAnimatorBool("backMovement", false);
            SetAnimatorBool("backIdle", false);
        }

        // Sağ ve sola hareket
        HandleSideMovement(KeyCode.D, "Right");
        HandleSideMovement(KeyCode.A, "Left");
    }

    private void HandleSideMovement(KeyCode key, string direction)
    {
        bool keyDown = Input.GetKeyDown(key);
        bool keyUp = Input.GetKeyUp(key);
        bool keyHold = Input.GetKey(key);

        SetAnimatorBool($"idle{direction}", keyDown || keyHold);
        SetAnimatorBool($"movement{direction}", keyHold);

        if (keyUp)
        {
            SetAnimatorBool($"idle{direction}", false);
            SetAnimatorBool($"movement{direction}", false);
        }
    }

    private void SetAnimatorFloat(string parameter, float value)
    {
        _animator.SetFloat(parameter, value, 0.1f, Time.deltaTime);
        photonView.RPC("SetAnimatorFloatRPC", RpcTarget.Others, parameter, value);
    }

    private void SetAnimatorBool(string parameter, bool value)
    {
        _animator.SetBool(parameter, value);
        photonView.RPC("SetAnimatorBoolRPC", RpcTarget.Others, parameter, value);
    }

    [PunRPC]
    void SetAnimatorFloatRPC(string parameter, float value)
    {
        _animator.SetFloat(parameter, value);
    }

    [PunRPC]
    void SetAnimatorBoolRPC(string parameter, bool value)
    {
        _animator.SetBool(parameter, value);
    }
}