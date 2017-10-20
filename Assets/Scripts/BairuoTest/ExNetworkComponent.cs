using System;
using UnityEngine;

public class ExNetworkComponent : ExNetworkBehaviour
{
    
    protected override void DestroyingReceive(object[] info)
    {
        Destroy(this);
    }
    

    protected override void OnDestroy()
    {
        if(Client.IsRoomServer()) Send("DefaultDestroying");
        networkScripts.Remove(netObject.NetID);
        netObject.SelfRemove();
    }
}