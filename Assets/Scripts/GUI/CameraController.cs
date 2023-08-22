using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Basic Settings")]
    [SerializeField, Range(0f, 10f)] private float smoothness;
    [SerializeField, Range(0f, 10f)] private float triggerRadius;
    [Header("Editor only")]
    [SerializeField] private Color gizmoColor;

    [SerializeField] private Player localPlayer;

    public Player LocalPlayer { get => localPlayer; set => localPlayer = value; }

    private bool isMoving = false;

    private void Update()
    {
#if UNITY_EDITOR
        if (localPlayer == null) Debug.LogWarning($"LocalPlayer on MainCamera ({this.gameObject}) is null!");
#endif

        Vector3 destination = localPlayer.transform.position;
        destination.z = transform.position.z;

        float distance = Vector3.Distance(transform.position, destination);
        if (distance > triggerRadius || isMoving)
        {
            transform.position = Vector3.Lerp(transform.position, destination, smoothness * Time.deltaTime);
            isMoving = (distance >= 0.01f);
        }
    }

    // Editor GUI

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
#endif
}
