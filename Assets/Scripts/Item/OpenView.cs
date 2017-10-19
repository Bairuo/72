using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenView : ExNetworkBehaviour
{
    public float time;
    void OnTriggerEnter2D(Collider2D x)
    {
        if(!Client.IsRoomServer()) return;
        
        var netx = x.gameObject.GetComponent<ExNetworkBehaviour>();
        if(netx == null) return;
        string id = netx.netObject.NetID;
        if(id == null || id != "Player-" + Client.instance.playerid) return;
        
        Component.FindObjectOfType<FogController>().MakeVisible(time);
        Destroy(this.gameObject);
    }
    
}
