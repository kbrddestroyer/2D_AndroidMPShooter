using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Basic Settings")]
    [SerializeField, Range(0f, 10f)] protected float speed;
    [Header("Required Objects")]

    protected Camera mainCamera;

    protected void Awake()
    {
        mainCamera = Camera.main;
    }

    protected void Update()
    {
        // Controls
        float speedX = Input.GetAxis("Horizontal");
        float speedY = Input.GetAxis("Vertical");

        transform.position += new Vector3(speedX, speedY, 0) * speed * Time.deltaTime;   // Main moving

    }
}
