using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class RigidbodyCharacterController : MonoBehaviour
{
    [Header("Objects And Components")]
    [SerializeField] private GameObject ViewObject;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CapsuleCollider col;
    [SerializeField] private GameObject middle;

    [Header("Layers")]
    [SerializeField] private LayerMask GroundLayer;

    [Header("View Settings")]
    [SerializeField] private float Deadzone;
    [SerializeField][Range(0.01f,10f)] private float VerticalViewSensitivity = 2.5f;
    [SerializeField][Range(0.01f, 10f)] private float HorizontalViewSensitivity = 2.5f;
    [SerializeField] private Vector2 VerticalLimits;
    public bool InvertControl = false;

    [Header("Move Settings")]
    [SerializeField] private float MoveSpeed;
    [SerializeField] private float JumpForce = 1f;
    [SerializeField] private float PlayerDrag = 4f;
    [SerializeField] private float AirSpeedMultiplier = 0.5f;

    [Header("Rotation Info")]
    [SerializeField] private float VerticalAngle;
    [SerializeField] private float HorizontalAngle;

    [Header("Move Info")]
    [SerializeField] private Vector2 MoveVectorInput;
    [SerializeField] private bool isGrounded;
    [SerializeField] private float GroundedTolerance;
    [SerializeField] private float MaxRayCastLength;

    PlayerInputActions playerInputActions;
    private float sensitivity_Vert;
    private float sensitivity_Hor;
    private Vector2 lastInput;
    private float jumpStrength;
    private float moveSpeed;


    private void Awake()
    {
        
        Cursor.lockState = CursorLockMode.Locked;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Jump.performed += PerformJump;

        if (rb == null)
        {
            rb = gameObject.GetComponent<Rigidbody>();
        }

        if (ViewObject == null)
            Debug.LogError("ViewObject not present.");

        MaxRayCastLength = ((CapsuleCollider)col).height / 2 + GroundedTolerance;
        moveSpeed = MoveSpeed;
    }

    private void FixedUpdate()
    {
        CheckIfGrounded();
        Body_ChangeDrag();
        PerformMove();
    }

    private void Update()
    {
        MoveVectorInput = CalculateMoveInputs();
        PerformRotations();

        if(playerInputActions.Player.Jump.IsPressed())
        {
            Invoke(nameof(PerformJump), 1f);
        }
    }

    private void PerformRotations()
    {
        sensitivity_Vert = VerticalViewSensitivity * 100;
        sensitivity_Hor = HorizontalViewSensitivity * 100;

        Vector2 inputVector = SmoothInput(playerInputActions.Player.View.ReadValue<Vector2>() * 0.1f);
        View_VertRotation(inputVector);
        Body_HorRotation(inputVector);
    }

    private void PerformMove()
    {
        Body_MovePlayer(MoveVectorInput);
    }

    private void PerformJump(InputAction.CallbackContext cont)
    {
        if(isGrounded)
        { 
            rb.AddForce(CalculateJumpStrength(), ForceMode.Impulse);
        }
    }

    private Vector2 CalculateMoveInputs()
    {
        Vector2 input = playerInputActions.Player.Move.ReadValue<Vector2>() * MoveSpeed;
        return input;
    }

    private Vector3 CalculateJumpStrength()
    {
        jumpStrength = JumpForce * rb.mass * 2;
        return jumpStrength * Vector3.up;
    }

    private Vector2 SmoothInput(Vector2 input)
    {
        if (input.x < Deadzone && input.x > -1 * Deadzone)
            input.x = 0;

        if (input.y < Deadzone && input.y > -1 * Deadzone)
            input.y = 0;

        float x = (input.x + lastInput.x) / 2;
        float y = (input.y + lastInput.y) / 2;

        lastInput = input;
        return new Vector2(x, y);
    }

    private void CheckIfGrounded()
    {
        isGrounded = Physics.Raycast(middle.transform.position, Vector3.down, col.height / 2 + GroundedTolerance);
    }

    private void View_VertRotation(Vector2 input)
    {
        float playerInput = input.y * sensitivity_Vert * Time.deltaTime;
        VerticalAngle = Mathf.Clamp(playerInput + VerticalAngle, VerticalLimits.x, VerticalLimits.y);

        if(InvertControl)
           ViewObject.transform.localRotation = Quaternion.Euler(VerticalAngle, 0, 0);
        else
            ViewObject.transform.localRotation = Quaternion.Euler(VerticalAngle * -1, 0, 0);
    }

    private void Body_HorRotation(Vector2 input)
    {
        float playerInput = input.x * sensitivity_Hor * Time.deltaTime;
        HorizontalAngle += playerInput;
        HorizontalAngle %= 360;

        gameObject.transform.rotation = Quaternion.Euler(0, HorizontalAngle, 0);
    }

    private void Body_MovePlayer(Vector2 input)
    {
        Vector3 input3 = new Vector3(input.x,0,input.y);
        rb.AddForce(transform.TransformVector(input3), ForceMode.Force);
    }

    private void Body_ChangeDrag()
    {
        if (!isGrounded)
        {
            MoveSpeed = moveSpeed * AirSpeedMultiplier;
            rb.drag = 0f;
        }
        else
        {
            MoveSpeed = moveSpeed;
            rb.drag = PlayerDrag;
        }
    }
}
