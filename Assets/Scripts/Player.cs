using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using Mirror;
using UnityEngine.InputSystem.Processors;
using Mirror.Examples.AdditiveScenes;
using UnityEngine.Pool;
using TMPro;

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(NetworkTransformReliable))]
public class Player : NetworkBehaviour
{
    [Header("Basic Settings")]
    [SerializeField, Range(0f, 10f)] protected float speed;
    [SerializeField, Range(0f, 10f)] protected float maxHp;
    [SerializeField, Range(0f, 10f)] protected float bulletsPerSecond;
    
    private TMP_Text coinsCounter;
    protected Camera mainCamera;

    protected PlayerInput playerInput;
    protected InputAction move;
    protected InputAction look;

    private float angle = 0f;
    private float hp;

    // Synchronised values
    [SyncVar(hook = nameof(ChangeCoins))] private int coins = 0;

    private void ChangeCoins(int _old, int _new) { this.coins = _new; }

    public int Coins {
        get => coins;

        set {
            coins = value;
            if (isOwned) coinsCounter.text = $"Coins: {coins}";
        }
    }

    public float HP { 
        get => hp; 
        set
        {
            // Deal damage, update GUI and play VFX
            hp -= value;
            if (hp <= 0) Death();
        }
    }

    protected virtual void Start()
    {
        mainCamera = Camera.main;

        if (isOwned) mainCamera.GetComponent<CameraController>().LocalPlayer = this; // Set LocalPlayer for camera to this device

        playerInput = GetComponent<PlayerInput>();
        move = playerInput.actions["Move"];
        look = playerInput.actions["Look"];

        coinsCounter = GameObject.Find("CoinsCounter").GetComponent<TMP_Text>();
    }

#if UNITY_EDITOR
    private void OnConnectedToServer()
    {
        Debug.LogWarning("Connected to server!");
    }
#endif

    private void Death()
    {
        // TODO
        if (isOwned) mainCamera.GetComponent<CameraController>().LocalPlayer = null;
        Destroy(this.gameObject);
    }

    protected virtual void Update()
    {
        if (isOwned)
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
}
