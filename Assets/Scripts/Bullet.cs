using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Pool;

public class Bullet : NetworkBehaviour
{
    [SerializeField, Range(1f, 10f)] private float damage;
    [SerializeField, Range(0f, 10f)] private float speed;

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
        transform.position += transform.up * speed * Time.deltaTime;
    }

    [Command(requiresAuthority = false)]
    private void CmdFree()
    {
        GameObject.FindObjectOfType<NetworkObjectPool>().Free(this.gameObject);
    }

    private void OnBecameInvisible()
    {
        CmdFree();
    }
}
