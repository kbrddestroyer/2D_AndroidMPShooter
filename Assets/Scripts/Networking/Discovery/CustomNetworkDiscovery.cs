using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;
using Mirror.Discovery;
using System.Net;

public class CustomNetworkDiscovery : NetworkDiscoveryBase<CustomDiscoveryRequest, CustomDiscoveryResponse>
{
    private static CustomNetworkDiscovery instance;
    public static CustomNetworkDiscovery Instance { get => instance; }
    public override void Start()
    {
        instance = this;
        base.Start();
        Debug.Log(secretHandshake);
        StartDiscovery();
        
    }

    public void Broadcast()
    {
        BroadcastDiscoveryRequest();
    }

    #region SERVER
    protected override void ProcessClientRequest(CustomDiscoveryRequest request, IPEndPoint endpoint)
    {
        Debug.Log("Got client request!");

        base.ProcessClientRequest(request, endpoint);
    }

    protected override CustomDiscoveryResponse ProcessRequest(CustomDiscoveryRequest request, IPEndPoint endpoint)
    {
        Debug.Log("Processing discovery request...");

        return new CustomDiscoveryResponse("Untitled server", CustomNetworkManager.Instance.numPlayers);
    }
    #endregion

    #region CLIENT
    protected override CustomDiscoveryRequest GetRequest()
    {
        Debug.Log("Creating discovery request!");
        return new CustomDiscoveryRequest();
    }

    protected override void ProcessResponse(CustomDiscoveryResponse response, IPEndPoint endpoint)
    {
        // Server reply
        Debug.Log($"Server has replied with following information:\n{response.ServerName} with {response.Players} on server");
    }
    #endregion
}
