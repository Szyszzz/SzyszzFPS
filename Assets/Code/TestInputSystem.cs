using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestInputSystem : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private GameObject ViewObject;

    [Header("View Settings")]
    [SerializeField] private float deadzone;
    [SerializeField][Range(0.01f,10f)] private float VerticalViewSensitivity = 2.5f;
    [SerializeField][Range(0.01f, 10f)] private float HorizontalViewSensitivity = 2.5f;
    [SerializeField] private float verticalSpeed = 1f;
    [SerializeField] private float horizontalSpeed = 1f;
    [SerializeField] private Vector2 VerticalLimits;
    public bool InvertControl = false;

    [Header("Rotation Info")]
    [SerializeField] private float VerticalAngle;
    [SerializeField] private float HorizontalAngle;

    PlayerInputActions playerInputActions;
    private float sensitivity_Vert;
    private float sensitivity_Hor;
    private Vector2 lastInput;


    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;

        sensitivity_Vert = VerticalViewSensitivity * 100;
        sensitivity_Hor = HorizontalViewSensitivity * 100;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        if (ViewObject == null)
            Debug.LogError("ViewObject not present.");
    }

    private void Update()
    {
        Vector2 inputVector = playerInputActions.Player.View.ReadValue<Vector2>();
        inputVector = SmoothInput(inputVector);
        View_VertRotation(inputVector);
        Body_HorRotation(inputVector);
    }

    private Vector2 SmoothInput(Vector2 input)
    {
        if (input.x < deadzone && input.x > -1 * deadzone)
            input.x = 0;

        if (input.y < deadzone && input.y > -1 * deadzone)
            input.y = 0;

        float x = (input.x + lastInput.x) / 2;
        float y = (input.y + lastInput.y) / 2;

        lastInput = input;
        return new Vector2(x, y);
    }

    private void View_VertRotation(Vector2 input)
    {
        float playerInput = input.y * verticalSpeed * sensitivity_Vert * Time.deltaTime;
        VerticalAngle = Mathf.Clamp(playerInput + VerticalAngle, VerticalLimits.x, VerticalLimits.y);

        if(InvertControl)
           ViewObject.transform.localRotation = Quaternion.Euler(VerticalAngle, 0, 0);
        else
            ViewObject.transform.localRotation = Quaternion.Euler(VerticalAngle * -1, 0, 0);
    }

    private void Body_HorRotation(Vector2 input)
    {
        float playerInput = input.x * horizontalSpeed * sensitivity_Hor * Time.deltaTime;
        HorizontalAngle += playerInput;
        HorizontalAngle %= 360;

        gameObject.transform.rotation = Quaternion.Euler(0, HorizontalAngle, 0);
    }
}
