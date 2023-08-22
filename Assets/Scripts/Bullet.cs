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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.HP -= damage;
        }
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
}
