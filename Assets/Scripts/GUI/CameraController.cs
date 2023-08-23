using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Basic Settings")]
    [SerializeField, Range(0f, 10f)] private float smoothness;
    [SerializeField, Range(0f, 10f)] private float triggerRadius;
    [Header("Required objects")]
    [SerializeField] private GameObject waitingForPlayerGUI;
    [SerializeField] private TMP_Text coinsCounter;
    [SerializeField] private TMP_Text HPCounter;
    [SerializeField] private GameObject endgamePopup;
    [SerializeField] private TMP_Text endgameText;
    [Header("Editor only")]
    [SerializeField] private Color gizmoColor;

    public int Coins { set => coinsCounter.text = $"Coins: {value}"; }
    public float HP { set => HPCounter.text = $"HP: {value}"; }
    
    public bool Endgame { set => endgamePopup.SetActive(value); }
    public bool Waiting { set => waitingForPlayerGUI.SetActive(value); }
    public string EndgameText { set => endgameText.text = value; }

    private Player localPlayer;

    public Player LocalPlayer { get => localPlayer; set => localPlayer = value; }

    private bool isMoving = false;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (localPlayer == null)
        {
            Debug.LogWarning($"LocalPlayer on MainCamera ({this.gameObject}) is null!");
            return;
        }

        Vector3 destination = localPlayer.transform.position;
        destination.z = transform.position.z;

        float distance = Vector3.Distance(transform.position, destination);
        if (distance > triggerRadius || isMoving)
        {
            transform.position = Vector3.Lerp(transform.position, destination, smoothness * Time.deltaTime);
            isMoving = (distance >= 0.01f);
        }
    }

    // VFX
    public void PlayDamageFX()
    {
        animator.SetTrigger("play");
    }

    public void BackToMainMenu()
    {
        if (NetworkClient.activeHost)
        {
            NetworkServer.Shutdown();
        }
        NetworkClient.Shutdown();

        Destroy(CustomNetworkManager.Instance.gameObject);
        SceneLoader.Instance.LoadScene("Loading");
        SceneLoader.Instance.LoadScene("Lobby");
    }

    // Editor GUI

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
#endif

    [Obsolete]
    public void ChangeWaitingForPlayerValue(bool value)
    {
        waitingForPlayerGUI.SetActive(value);
    }
}
