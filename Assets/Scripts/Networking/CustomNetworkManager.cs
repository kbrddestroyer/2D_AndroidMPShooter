using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

public class CustomNetworkManager : NetworkManager
{
    public void setDestinationIP(string destination)
    {
        this.networkAddress = destination;
    }

    public void ConnectHost()
    {
        ConnectHost();
    }

    public void ConnectPlayer()
    {
        ConnectPlayer();
    }
}
