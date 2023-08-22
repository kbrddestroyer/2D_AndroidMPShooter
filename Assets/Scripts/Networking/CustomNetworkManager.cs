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

    private List<Player> players = new List<Player>();

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

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject player = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, player);

        players.Add(player.GetComponent<Player>());
        
        foreach (ShootingPlayer _player in players)
        {
            _player.Activated = (players.Count > 0);   
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        int correction = 1;
        foreach (ShootingPlayer player in players)
            if (player.GetComponent<NetworkIdentity>().connectionToClient == conn)
            {
                players.Remove(player);
                correction = 0;
            }
            else player.Activated = (players.Count - correction > 0);
        base.OnServerDisconnect(conn);
    }
}
