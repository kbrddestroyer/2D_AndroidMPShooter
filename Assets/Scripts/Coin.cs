using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

public class Coin : NetworkBehaviour
{
    [SerializeField, Range(0f, 10f)] private float delay;

    private new Renderer renderer;
    private bool canBeCollected = true;

    public void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(delay);
        RpcSetActive(true);
    }

    [ClientRpc]
    private void RpcSetActive(bool value)
    {
        canBeCollected = value;
        renderer.enabled = value;
    }

    [Command(requiresAuthority = false)]
    private void CmdRespawn()
    {
        StartCoroutine(Respawn());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        Debug.Log(player);
        if (player != null && canBeCollected)
        {
            player.Coins++;
            RpcSetActive(false);
            CmdRespawn();
        }
    }
}
