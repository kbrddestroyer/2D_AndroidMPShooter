using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Basic Settings")]
    [SerializeField, Range(0f, 10f)] protected float speed;
    [Header("Required Objects")]

    protected Camera mainCamera;
    protected PlayerInput playerInput;

    private InputAction move;
    private InputAction look;
    private float angle = 0f;

    protected void Awake()
    {
        mainCamera = Camera.main;
        playerInput = GetComponent<PlayerInput>();
        move = playerInput.actions["Move"];
        look = playerInput.actions["Look"];
    }

    protected void Update()
    {
        // Controls

        // 1. Movement controls
        Vector2 moveControls = move.ReadValue<Vector2>();

        transform.position += new Vector3(moveControls.x, moveControls.y, 0) * speed * Time.deltaTime;   // Main moving

        // 2. Rotation controls
        if (look.inProgress)
        {
            Vector2 lookControls = look.ReadValue<Vector2>();
            angle = Vector2.Angle(Vector2.up, lookControls) * ((lookControls.x > 0) ? -1 : 1);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
