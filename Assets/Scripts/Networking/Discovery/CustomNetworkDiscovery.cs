using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror
using Mirror.Discovery;
using System.Net;

public class CustomNetworkDiscovery : NetworkDiscoveryBase<CustomDiscoveryRequest, CustomDiscoveryResponse>
{
    #region SERVER
    protected override CustomDiscoveryResponse ProcessRequest(CustomDiscoveryRequest request, IPEndPoint endpoint)
    {
        throw new System.NotImplementedException();
    }
    #endregion
    #region CLIENT
    protected override void ProcessResponse(CustomDiscoveryResponse response, IPEndPoint endpoint)
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
