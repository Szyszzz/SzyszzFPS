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
    [SerializeField][Range(0.01f,10f)] private float VerticalViewSensitivity = 2.5f;
    [SerializeField][Range(0.01f, 10f)] private float HorizontalViewSensitivity = 2.5f;
    [SerializeField] private Vector2 VerticalLimits;
    public bool InvertControl = false;

    [Header("Rotation Info")]
    [SerializeField] private float VerticalAngle;

    PlayerInputActions playerInputActions;
    private float sensitivity_Vert;
    private float sensitivity_Hor;


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
        View_VertRotation(inputVector);
    }

    private void View_VertRotation(Vector2 input)
    {
        float playerInput = input.y * sensitivity_Vert * Time.deltaTime;
        VerticalAngle = Mathf.Clamp(playerInput + VerticalAngle, VerticalLimits.x, VerticalLimits.y);

        if(InvertControl)
           ViewObject.transform.rotation = Quaternion.Euler(VerticalAngle, 0, 0);
        else
            ViewObject.transform.rotation = Quaternion.Euler(VerticalAngle * -1, 0, 0);
    }
}
