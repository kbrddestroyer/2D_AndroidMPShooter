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
public class Player : NetworkBehaviour, IShooting
{
    [Header("Basic Settings")]
    [SerializeField, Range(0f, 10f)] protected float speed;
    [SerializeField, Range(0f, 10f)] protected float maxHp;
    [SerializeField, Range(0f, 10f)] protected float bulletsPerSecond;
    [Header("Required Objects")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletSpawnPoint;
    
    private TMP_Text coinsCounter;
    protected Camera mainCamera;
    
    private PlayerInput playerInput;
    private InputAction move;
    private InputAction look;

    private float angle = 0f;
    private float hp;
    private float shootingTimePassed = 0f;
    [SyncVar(hook = nameof(ChangeCoins))] private int coins = 0;

    private void ChangeCoins(int _old, int _new) { this.coins = _new; }

    public int Coins {
        get => coins;

        set {
            coins = value;
            coinsCounter.text = $"Coins: {coins}";
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

    protected void Start()
    {
        mainCamera = Camera.main;

        if (isOwned) mainCamera.GetComponent<CameraController>().LocalPlayer = this; // Set LocalPlayer for camera to this device

        playerInput = GetComponent<PlayerInput>();
        move = playerInput.actions["Move"];
        look = playerInput.actions["Look"];

        coinsCounter = GameObject.Find("CoinsCounter").GetComponent<TMP_Text>();
    }

    [Command]
    private void CmdCreateBullet()
    {
        Bullet bullet = CreateBullet();
        NetworkServer.Spawn(bullet.gameObject);
    }

    private Bullet CreateBullet()
    {
        Bullet _bullet = Instantiate(bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation).GetComponent<Bullet>();
        return _bullet;
    }

    [ClientCallback]
    public void Shoot()
    {
        shootingTimePassed += Time.deltaTime;
        if (shootingTimePassed >= 1f / bulletsPerSecond)
        {
            CmdCreateBullet();
            shootingTimePassed = 0f;
        }
    }

    private void Death()
    {
        // TODO
        if (isOwned) mainCamera.GetComponent<CameraController>().LocalPlayer = null;
        Destroy(this.gameObject);
    }

    protected void Update()
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

                Debug.Log(Vector2.Distance(Vector2.zero, lookControls));
                if (Vector2.Distance(Vector2.zero, lookControls) > 0.75)
                {
                    Shoot();
                }
            }
        }
    }
}
