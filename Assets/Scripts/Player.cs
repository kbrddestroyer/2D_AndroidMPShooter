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

    [SyncVar] private Color color;
    public Color PlayerColor {
        [Server]
        set
        {
            color = value;
            ChangeColor(color);
        }
    }

    protected Camera mainCamera;
    protected CameraController cameraController;

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
            if (isOwned) cameraController.Coins = value;
        }
    }

    public float HP { 
        get => hp; 
        set
        {
            // Deal damage, update GUI and play VFX
            if (isOwned)
            {
                cameraController.HP = value;
                if (hp > value) cameraController.PlayDamageFX();
            }
            hp = value;
            if (hp <= 0) Death();
        }
    }

    protected virtual void Start()
    {
        mainCamera = Camera.main;
        cameraController = mainCamera.GetComponent<CameraController>();

        if (isOwned) cameraController.LocalPlayer = this; // Set LocalPlayer for camera to this device

        playerInput = GetComponent<PlayerInput>();
        move = playerInput.actions["Move"];
        look = playerInput.actions["Look"];
        HP = maxHp;
        ChangeColorLocal(color);
    }

    [ClientRpc]
    public void ChangeColor(Color color)
    {
        ChangeColorLocal(color);
    }

    private void ChangeColorLocal(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
        foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.color = color;
        }
    }

    [Command]
    private void CmdDestroy()
    {
        CustomNetworkManager.Instance.RemovePlayerFromList(this);
        NetworkServer.Destroy(this.gameObject);
    }

    private void Death()
    {
        // TODO
        if (isOwned)
        {
            cameraController.LocalPlayer = FindObjectOfType<Player>();
            cameraController.Endgame = true;
            cameraController.EndgameText = $"You lose!\nYou've collected {coins} coin{((coins > 1) ? "s!" : '!')}";
        }
        CmdDestroy();
    }

    [TargetRpc]
    public void Win(NetworkConnectionToClient conn)
    {
        if (isOwned)
        {
            cameraController.Endgame = true;
            cameraController.EndgameText = $"You win!\nYou've collected {coins} coin{((coins > 1) ? "s!" : '!')}";
        }
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
