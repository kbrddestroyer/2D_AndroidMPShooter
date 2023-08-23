using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;
using System;

public class CustomDiscoveryResponse : NetworkMessage
{
    #region SERVER
    
    private string serverName;
    private int players;

    public string ServerName { get => serverName; }
    public int Players { get => players; }

    public CustomDiscoveryResponse()
    {
        serverName = "Untitled server";
        players = 0;
    }

    public CustomDiscoveryResponse(string serverName, int players)
    {
        this.serverName = serverName;
        this.players = players;
    }

    #endregion
}
