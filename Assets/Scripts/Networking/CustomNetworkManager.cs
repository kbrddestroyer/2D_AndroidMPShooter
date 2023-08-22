using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;
using Mirror.Discovery;

public class CustomNetworkManager : NetworkManager
{
    private static CustomNetworkManager instance;
    private NetworkDiscovery networkDiscovery;
    public static CustomNetworkManager Instance { get => instance; }

    public override void Awake()
    {
        if (instance == null) instance = this;
        base.Awake();

        networkDiscovery = GetComponent<NetworkDiscovery>();
    }

    public void SetDestinationIP(string destination)
    {
        this.networkAddress = destination;
    }
}
