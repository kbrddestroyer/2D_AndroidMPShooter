using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Pool;

public class Bullet : NetworkBehaviour
{
    [SerializeField, Range(1f, 10f)] private float damage;
    [SerializeField, Range(0f, 10f)] private float speed;
    [SerializeField, Range(0f, 10f)] private float lifetime;

    private float lifetimeCurrent;

    public Transform StartPosition {
        set
        {
            transform.position = value.position;
            transform.rotation = value.rotation;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.HP -= damage;
        }
        CmdFree();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.HP -= damage;
        }
        CmdFree();
    }

    [ClientRpc]
    public void setActivated(bool isActive)
    {
        this.gameObject.SetActive(isActive);
    }

    private void Update()
    {
        if (isServer)
        {
            lifetimeCurrent += Time.deltaTime;
            if (lifetimeCurrent >= lifetime) ServerFree();
        }
        transform.position += transform.up * speed * Time.deltaTime;
    }

    [Server]
    private void ServerFree()
    {
        lifetimeCurrent = 0;
        NetworkObjectPool.Instance.Free(gameObject);
    }

    [Command(requiresAuthority = false)]
    private void CmdFree() { ServerFree(); }
}
