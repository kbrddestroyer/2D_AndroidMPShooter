using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;
using Mirror.Discovery;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private Color[] colors;
    [Header("Debug")]
    [SerializeField, Range(0f, 10f)] private int minimumRequiredPlayers;

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

    public override void OnClientError(TransportError error, string reason)
    {
        Debug.LogWarning(reason);
        SceneLoader.Instance.LoadScene("Lobby");
        
        base.OnClientError(error, reason);
    }

    #region SERVER
    public void RemovePlayerFromList(Player player)
    {
        players.Remove(player);
        if (players.Count == 1)
        {
            players[0].Win(players[0].GetComponent<NetworkIdentity>().connectionToClient);
        }
        else
            foreach (ShootingPlayer _player in players)
                _player.Activated = (players.Count > minimumRequiredPlayers);
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Transform startPos = GetStartPosition();
        GameObject player = Instantiate(playerPrefab, startPos.position, startPos.rotation);
        Player playerComponent = player.GetComponent<Player>();
        
        NetworkServer.AddPlayerForConnection(conn, player);

        players.Add(playerComponent);
        playerComponent.PlayerColor = colors[players.Count - 1];
        foreach (ShootingPlayer _player in players)
        {
            _player.Activated = (players.Count > minimumRequiredPlayers);   
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        int correction = 1;
        foreach (ShootingPlayer player in players)
            if (player.GetComponent<NetworkIdentity>().connectionToClient == conn)
            {
                NetworkServer.Destroy(player.gameObject);
                players.Remove(player);
                correction = 0;
            }
            else player.Activated = (players.Count - correction > minimumRequiredPlayers);
        NetworkServer.DestroyPlayerForConnection(conn);
    }
    #endregion
}
