using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class NetworkObjectPool : NetworkBehaviour
{
    public static NetworkObjectPool instance;

    [Header("Pool size")]
    [SerializeField, Range(0, 100)] private int startSize;
    [SerializeField, Range(0, 100)] private int maxSize;
    [Header("Debug")]
    [SerializeField] private Queue<GameObject> queue;
    [SerializeField] private NetworkIdentity prefab;

    private int freeCount = 0;

    private void Start()
    {
        if (!instance) instance = this;
        Initialise();
    }

    [ClientRpc]
    private void SetObjectState(GameObject ob, bool value)
    {
        ob.SetActive(value);
    }

    [ServerCallback]
    private GameObject Add()
    {
        GameObject ob = Instantiate(prefab.gameObject, transform.position, transform.rotation);
        NetworkServer.Spawn(ob);
        SetObjectState(ob, false);
        return ob;
    }

    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        GameObject next = (queue.Count > 0) ? queue.Dequeue() : Add();
        next.transform.position = position;
        next.transform.rotation = rotation;
        SetObjectState(next, true);
        return next;
    }

    public void Free(GameObject poolObject)
    {
        if (queue.Count > maxSize) NetworkServer.Destroy(poolObject);
        else
        {
            SetObjectState(poolObject, false);
            queue.Enqueue(poolObject);
        }
    }

    [Command(requiresAuthority = false)]
    private void Initialise()
    {
        queue = new Queue<GameObject>();
        for (int i = 0; i < startSize; i++)
        {
            queue.Enqueue(Add());
        }
    }
}
