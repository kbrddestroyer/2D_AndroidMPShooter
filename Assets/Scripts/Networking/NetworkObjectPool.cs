using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class NetworkObjectPool : NetworkBehaviour
{
    private static NetworkObjectPool instance;
    public static NetworkObjectPool Instance { get => instance; }

    [Header("Pool size")]
    [SerializeField, Range(0, 100)] private int startSize;
    [SerializeField, Range(0, 100)] private int maxSize;
    [Header("Debug")]
    [SerializeField] private Queue<GameObject> queue;
    [SerializeField] private NetworkIdentity prefab;

    #region CLIENT
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

    [ClientRpc]
    private void MoveObject(GameObject ob, Vector3 position, Quaternion rotation)
    {
        ob.transform.position = position;
        ob.transform.rotation = rotation;
    }
    #endregion

    #region SERVER

    [ServerCallback]
    private GameObject Add()
    {
        GameObject ob = Instantiate(prefab.gameObject, transform.position, transform.rotation);
        NetworkServer.Spawn(ob);
        SetObjectState(ob, false);
        return ob;
    }

    [ServerCallback]
    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        GameObject next = (queue.Count > 0) ? queue.Dequeue() : Add();
        MoveObject(next, position, rotation);
        SetObjectState(next, true);
        return next;
    }

    [ServerCallback]
    public void Free(GameObject poolObject)
    {
        SetObjectState(poolObject, false);
        queue.Enqueue(poolObject);
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

    #endregion
}
